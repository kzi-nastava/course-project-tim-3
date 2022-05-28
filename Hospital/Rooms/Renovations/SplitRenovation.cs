using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using HospitalSystem.Utils;

namespace HospitalSystem;

public class SplitRenovation
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateRange BusyRange { get; set; }
    public string SplitRoomLocation { get; set; }
    public string SplitToFirstLocation { get; set; }
    public string SplitToSecondLocation { get; set; }
    public bool IsDone { get; set; }

    public SplitRenovation(DateRange busyRange, string splitRoomLocation,
        string splitToFirstLocation, string splitToSecondLocation)
    {
        if (splitToFirstLocation == splitToSecondLocation)
        {
            throw new ArgumentException("Nope, can't have same location for both.");
        }
        Id = ObjectId.GenerateNewId();
        BusyRange = busyRange;
        SplitRoomLocation = splitRoomLocation;
        SplitToFirstLocation = splitToFirstLocation;
        SplitToSecondLocation = splitToSecondLocation;
        IsDone = false;
    }
}