using MongoDB.Driver;

namespace HospitalSystem;

public class RoomRepository
{
    private MongoClient _dbClient;

    public RoomRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }

    private IMongoCollection<Room> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Room>("rooms");
    }

    public IQueryable<Room> GetAll()
    {
        return 
            from room in GetMongoCollection().AsQueryable()
            where room.Active && !room.Deleted
            select room;
    }

    public bool Delete(string location)
    {
        var rooms = GetMongoCollection();
        return rooms.UpdateOne(room => room.Location == location && !room.Deleted,
            Builders<Room>.Update.Set("Deleted", true)).ModifiedCount == 1;
    }

    public void Add(Room newRoom)
    {
        var rooms = GetMongoCollection();
        rooms.InsertOne(newRoom);
    }

    public void AddInactive(Room newRoom)
    {
        newRoom.Active = false;
        var rooms = GetMongoCollection();
        rooms.ReplaceOne(room => room.Location == newRoom.Location && !room.Deleted && !room.Active, 
            newRoom, new ReplaceOptions {IsUpsert = true});
    }

    public bool DoesExist(string location)
    {
        return GetMongoCollection().Find(room => room.Location == location && !room.Deleted).Any();
    }

    public void Replace(Room changingRoom)
    {
        var rooms = GetMongoCollection();
        rooms.ReplaceOne(room => room.Id == changingRoom.Id && !room.Deleted, changingRoom);
    }

    public void Activate(string location)
    {
        var rooms = GetMongoCollection();
        rooms.UpdateOne(room => room.Location == location && !room.Deleted, Builders<Room>.Update.Set("Active", true));
    }

    public void Deactivate(string location)
    {
        // TODO: check if room still exists by this time... or stop delete if renovating
        var rooms = GetMongoCollection();
        rooms.UpdateOne(room => room.Location == location && !room.Deleted, Builders<Room>.Update.Set("Active", false));
    }
}