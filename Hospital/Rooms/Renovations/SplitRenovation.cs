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

    public SplitRenovation(string splitRoomLocation, DateRange busyRange, Room firstNewRoom, Room secondNewRoom)
    {
        Id = ObjectId.GenerateNewId();
        SplitRoomLocation = splitRoomLocation;
        SplitToFirstLocation = firstNewRoom.Location;
        SplitToSecondLocation = secondNewRoom.Location;
        BusyRange = busyRange;
        IsDone = false;
    }
}