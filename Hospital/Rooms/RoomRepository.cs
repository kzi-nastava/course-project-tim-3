using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace Hospital
{
    public class RoomRepository
    {
        private MongoClient _dbClient;

        public RoomRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Room> GetRooms()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Room>("rooms");
        }

        public IMongoQueryable<Room> GetQueryableRooms()
        {
            return GetRooms().AsQueryable();
        }

        public bool DeleteRoom(string location)
        {
            var rooms = GetRooms();
            return rooms.DeleteOne(room => room.Location == location).DeletedCount == 1;
        }

        public bool DeleteRoom(ObjectId id)
        {
            var rooms = GetRooms();
            return rooms.DeleteOne(room => room.Id == id).DeletedCount == 1;
        }

        public void AddRoom(Room newRoom)
        {
            var rooms = GetRooms();
            rooms.ReplaceOne(room => room.Location == newRoom.Location, newRoom, new ReplaceOptions {IsUpsert = true});
        }

        public void UpdateRoom(Room changingRoom)
        {
            var rooms = GetRooms();
            rooms.ReplaceOne(room => room.Id == changingRoom.Id, changingRoom, new ReplaceOptions {IsUpsert = true});
        }

        public Room? GetRoom(string location)
        {
            var rooms = GetRooms();
            return rooms.Find(room => room.Location == location).FirstOrDefault();
        }

        public Room? GetRoom(ObjectId id)
        {
            var rooms = GetRooms();
            return rooms.Find(room => room.Id == id).FirstOrDefault();
        }

        // todo: update
    }
}