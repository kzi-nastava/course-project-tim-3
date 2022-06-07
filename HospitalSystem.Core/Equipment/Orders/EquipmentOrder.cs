using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem.Core;

public class EquipmentOrder
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string ToStockLocation { get; set; }
    public string Name { get; set; }
    public EquipmentType Type { get; set; }
    public int Count { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime EndTime { get; set; }
    public bool IsDone { get; set; }

    public EquipmentOrder(string name, int count, EquipmentType type, 
        DateTime endTime, string toStockLocation)
    {
        Id = ObjectId.GenerateNewId();
        ToStockLocation = toStockLocation;
        Name = name;
        Count = count;
        EndTime = endTime;
        Type = type;
        IsDone = false;
    }
}