using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalSystem.Core.Rooms;

public enum RoomType
{
    REST,
    OPERATION,
    CHECKUP,
    OTHER,
    STOCK
}

public class Room
{
    // todo: naming
    [BsonId]
    public ObjectId Id {get; set;}

    [BsonRepresentation(BsonType.String)]
    public RoomType Type {get; set;}
    public string Location {get; set;}
    public string Name {get; set;}
    public bool Active { get; set; }
    public bool Deleted { get; set; }

    public Room(string location, string name, RoomType type)
    {
        Id = ObjectId.GenerateNewId();
        Location = location;
        Name = name;
        Type = type;
        Active = true;
        Deleted = false;
    }

    public override string ToString()
    {
        return Location + "Type: " + Type;
    }
}