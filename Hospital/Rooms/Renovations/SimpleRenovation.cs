using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem;

public class SimpleRenovation
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime StartTime { get; set; }
    public string RoomLocation { get; set; }
    public bool IsDone { get; set; }

    public SimpleRenovation(string roomLocation, DateTime startTime, DateTime endTime)
    {
        if (endTime < startTime)
        {
            throw new ArgumentException("End time can not be before start time");
        }
        Id = ObjectId.GenerateNewId();
        RoomLocation = roomLocation;
        EndTime = endTime;
        StartTime = startTime;
        IsDone = false;
    }
}