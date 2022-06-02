using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem.Core;

public class EquipmentRelocation
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string FromRoomLocation { get; set; }
    public string ToRoomLocation { get; set; }
    public string Name { get; set; }
    public EquipmentType Type { get; set; }
    public int Count { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsDone { get; set; }

    public EquipmentRelocation(string name, int count, EquipmentType type, 
        DateTime endTime, string fromRoomLocation, string toRoomLocation)
    {
        Id = ObjectId.GenerateNewId();
        FromRoomLocation = fromRoomLocation;
        ToRoomLocation = toRoomLocation;
        Name = name;
        Count = count;
        EndTime = endTime;
        Type = type;
        IsDone = false;
    }
}