using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem;

public class SplitRenovation
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string SplitRoomLocation { get; set; }
    public string SplitToFirstLocation { get; set; }
    public string SplitToSecondLocation { get; set; }
    public bool IsDone { get; set; }

    public SplitRenovation(string splitRoomLocation, DateTime startTime, DateTime endTime, Room firstNewRoom, Room secondNewRoom)
    {
        if (endTime < startTime)
        {
            throw new ArgumentException("End time can not be before start time");
        }
        Id = ObjectId.GenerateNewId();
        SplitRoomLocation = splitRoomLocation;
        SplitToFirstLocation = firstNewRoom.Location;
        SplitToSecondLocation = secondNewRoom.Location;
        EndTime = endTime;
        StartTime = startTime;
        IsDone = false;
    }
}