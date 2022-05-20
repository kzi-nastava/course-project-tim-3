using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using HospitalSystem.Utils;

namespace HospitalSystem;

public class MergeRenovation
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateRange BusyRange { get; set; }
    public string FirstLocation { get; set; }
    public string SecondLocation { get; set; }
    public string MergeToLocation { get; set; }
    public bool IsDone { get; set; }

    public MergeRenovation(DateRange busyRange, string firstLocation, string secondLocation, string mergeToLocation)
    {
        Id = ObjectId.GenerateNewId();
        BusyRange = busyRange;
        FirstLocation = firstLocation;
        SecondLocation = secondLocation;
        MergeToLocation = mergeToLocation;
        IsDone = false;
    }
}