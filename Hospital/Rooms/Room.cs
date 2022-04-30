using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hospital
{
    public class Room
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
        public string Name {get;}

        public Room(string location, string name, RoomType type)
        {
            Id = ObjectId.GenerateNewId();
            Location = location;
            Name = name;
            Type = type;
        }
    }
}