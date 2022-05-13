using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Hospital;

public class MergeRenovation
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string FirstLocation { get; set; }
    public string SecondLocation { get; set; }
    public string MergeToLocation { get; set; }
    public bool IsDone { get; set; }

    public MergeRenovation(DateTime startTime, DateTime endTime, string firstLocation, string secondLocation, string mergeToLocation)
    {
        if (endTime < startTime)
        {
            throw new ArgumentException("End time can not be before start time");
        }
        Id = ObjectId.GenerateNewId();
        StartTime = startTime;
        EndTime = endTime;
        FirstLocation = firstLocation;
        SecondLocation = secondLocation;
        MergeToLocation = mergeToLocation;
        IsDone = false;
    }
}