using System.Linq.Expressions;
using MongoDB.Driver;

namespace HospitalSystem.Core.Rooms;

public class RoomRepository : IRoomRepository
{
    private MongoClient _dbClient;

    public RoomRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<Room> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Room>("rooms");
    }

    private IQueryable<Room> GetAll()
    {
        return
            from room in GetMongoCollection().AsQueryable()
            where !room.Deleted
            select room;
    }

    public IQueryable<Room> GetActive()
    {
        return
            from room in GetMongoCollection().AsQueryable()
            where !room.Deleted && room.Active
            select room;
    }

    public void Insert(Room room)
    {
        GetMongoCollection().InsertOne(room);
    }

    public bool Delete(string location)
    {
        return GetMongoCollection().UpdateOne(room => room.Location == location && !room.Deleted,
            Builders<Room>.Update.Set("Deleted", true)).ModifiedCount == 1;
    }

    public Room Get(string location)
    {
        return
            (from room in GetAll()
            where room.Location == location
            select room).First();
    }

    public void Replace(Room replacing, Expression<Func<Room, bool>> filter)
    {
        GetMongoCollection().ReplaceOne(filter, replacing);
    }

    public void Upsert(Room newRoom, Expression<Func<Room, bool>> filter)
    {
        GetMongoCollection().ReplaceOne(filter, newRoom, new ReplaceOptions {IsUpsert = true});
    }

    public bool DoesExist(string location)
    {
        return
            (from room in GetAll()
            where room.Location == location
            select room).Any();
    }

    public IQueryable<Room> GetStocks()
    {
        return 
            from stock in GetAll()
            where stock.Type == RoomType.STOCK
            select stock;
    }
}