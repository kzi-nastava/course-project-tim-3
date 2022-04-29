using MongoDB.Driver;

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

        public bool DeleteRoom(string location)
        {
            var rooms = GetRooms();
            return rooms.DeleteOne(room => room.Location == location).DeletedCount == 1;
        }

        public void AddRoom(Room newRoom)
        {
            var rooms = GetRooms();
            rooms.ReplaceOne(room => room.Location == newRoom.Location, newRoom, new ReplaceOptions {IsUpsert = true});
        }

        public Room? GetRoom(string location)
        {
            var rooms = GetRooms();
            return rooms.Find(room => room.Location == location).FirstOrDefault();
        }

        // todo: update
    }
}