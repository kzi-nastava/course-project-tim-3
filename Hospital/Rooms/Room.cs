using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hospital
{
    public abstract class Room
    {
        public enum RoomType
        {
            REST,
            OPERATION,
            EXAMINATION,
            OTHER,
            STOCK
        }

        [BsonId]
        public ObjectId Id {get;}

        [BsonRepresentation(BsonType.String)]
        public RoomType Type {get;}
        public string Location {get;}

        public Room(string location, RoomType type)
        {
            Id = ObjectId.GenerateNewId();
            Location = location;
            Type = type;
        }
    }
}