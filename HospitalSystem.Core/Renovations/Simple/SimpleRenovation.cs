using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

public class SimpleRenovation
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateRange BusyRange { get; set; }
    public string RoomLocation { get; set; }
    public bool IsDone { get; set; }

    public SimpleRenovation(string roomLocation, DateRange busyRange)
    {
        Id = ObjectId.GenerateNewId();
        RoomLocation = roomLocation;
        BusyRange = busyRange;
        IsDone = false;
    }
}