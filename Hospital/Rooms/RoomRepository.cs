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
        return GetCollection().AsQueryable();
    }

    public bool Delete(string location)
    {
        var rooms = GetCollection();
        return rooms.DeleteOne(room => room.Location == location).DeletedCount == 1;
    }

    public bool Delete(ObjectId id)
    {
        var rooms = GetCollection();
        return rooms.DeleteOne(room => room.Id == id).DeletedCount == 1;
    }

    public void Add(Room newRoom)
    {
        var rooms = GetCollection();
        rooms.ReplaceOne(room => room.Location == newRoom.Location, newRoom, new ReplaceOptions {IsUpsert = true});
    }

    public void Replace(Room changingRoom)
    {
        var rooms = GetCollection();
        rooms.ReplaceOne(room => room.Id == changingRoom.Id, changingRoom);
    }

    public Room? Get(string location)
    {
        var rooms = GetCollection();
        return rooms.Find(room => room.Location == location).FirstOrDefault();
    }

    public Room? Get(ObjectId id)
    {
        var rooms = GetCollection();
        return rooms.Find(room => room.Id == id).FirstOrDefault();
    }

    // todo: update
}