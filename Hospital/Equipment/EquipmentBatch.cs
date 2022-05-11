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
    public string RoomLocation { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }  // TODO: add AvailableEquipment
    [BsonRepresentation(BsonType.String)]
    public EquipmentType Type { get; set; }
    
    // TODO: add min count 1
    public EquipmentBatch(string roomLocation, string name, int count, EquipmentType type)
    {
        Id = ObjectId.GenerateNewId();
        Name = name;
        Count = count;
        Type = type;
        RoomLocation = roomLocation;
    }

    public void MergeWith(EquipmentBatch other)
    {
        if (other.Name != Name || other.RoomLocation != RoomLocation)
            throw new Exception("NOPE, NOT THE SAME EQUIP");  // TODO: change this exception
        Count += other.Count;
    }

    public void Remove(EquipmentBatch other)
    {
        if (other.Name != Name || other.RoomLocation != RoomLocation)
            throw new Exception("NOPE, NOT THE SAME EQUIP");  // TODO: change this exception
        if (other.Count > Count)
            throw new Exception("NOPE, CAN'T REMOVE MORE THAN YOU HAVE");  // TODO: change this exception
        Count -= other.Count;
    }
}