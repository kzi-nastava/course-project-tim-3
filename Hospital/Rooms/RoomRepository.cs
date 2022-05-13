using MongoDB.Driver;
using MongoDB.Bson;

namespace Hospital;

public class RoomRepository
{
    private MongoClient _dbClient;

    public RoomRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }

    private IMongoCollection<Room> GetCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Room>("rooms");
    }

    public IQueryable<Room> GetAll()
    {
        return 
            from room in GetCollection().AsQueryable()
            where room.Active && !room.Deleted
            select room;
    }

    public bool Delete(string location)
    {
        var rooms = GetCollection();
        return rooms.UpdateOne(room => room.Location == location && !room.Deleted,
            Builders<Room>.Update.Set("Deleted", true)).ModifiedCount == 1;
    }

    public void Add(Room newRoom)
    {
        var rooms = GetCollection();
        rooms.InsertOne(newRoom);
    }

    public void AddInactive(Room newRoom)
    {
        newRoom.Active = false;
        var rooms = GetCollection();
        rooms.ReplaceOne(room => room.Location == newRoom.Location && !room.Deleted && !room.Active, 
            newRoom, new ReplaceOptions {IsUpsert = true});
    }

    public bool DoesExist(string location)
    {
        return GetCollection().Find(room => room.Location == location && !room.Deleted).Any();
    }

    public void Replace(Room changingRoom)
    {
        var rooms = GetCollection();
        rooms.ReplaceOne(room => room.Id == changingRoom.Id && !room.Deleted, changingRoom);
    }

    public void Activate(string location)
    {
        var rooms = GetCollection();
        rooms.UpdateOne(room => room.Location == location && !room.Deleted, Builders<Room>.Update.Set("Active", true));
    }

    public void Deactivate(string location)
    {
        // TODO: check if room still exists by this time... or stop delete if renovating
        var rooms = GetCollection();
        rooms.UpdateOne(room => room.Location == location && !room.Deleted, Builders<Room>.Update.Set("Active", false));
    }
}