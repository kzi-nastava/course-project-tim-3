using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Hospital;

public enum EquipmentType
{
    CHECKUP,
    OPERATION,
    FURNITURE,
    HALLWAY
}

public class EquipmentBatch
{
    [BsonId]
    public ObjectId Id { get; set; }
    public MongoDBRef Room { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    [BsonRepresentation(BsonType.String)]
    public EquipmentType Type { get; set; }
    
    public EquipmentBatch(Room room, string name, int count, EquipmentType type)
    {
        Id = ObjectId.GenerateNewId();
        Name = name;
        Count = count;
        Type = type;
        Room = new MongoDBRef("rooms", room.Id);
    }

    public void MergeWith(EquipmentBatch other)
    {
        if (other.Name != Name || other.Room.Id != Room.Id)
            throw new Exception("NOPE, NOT THE SAME EQUIP");  // TODO: change this
        Count += other.Count;
    }
}