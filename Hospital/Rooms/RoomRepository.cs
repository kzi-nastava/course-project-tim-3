using System.Linq.Expressions;
using MongoDB.Driver;

namespace HospitalSystem;

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

    public IQueryable<Room> GetAll()
    {
        return
            from room in GetMongoCollection().AsQueryable()
            where !room.Deleted
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

}