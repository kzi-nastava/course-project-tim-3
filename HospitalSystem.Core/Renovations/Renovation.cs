using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core.Renovations;

public class Renovation
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateRange BusyRange { get; set; }
    public List<string> NewLocations { get; set; }
    public List<string> OldLocations { get; set; }
    public bool IsDone { get; set; }

    public Renovation(DateRange busyRange, List<string> oldLocations, List<string> newLocations)
    {
        if (newLocations.Count == 0)
        {
            throw new RenovationException("Must have at least one new location.");
        }
        if (oldLocations.Count == 0)
        {
            throw new RenovationException("Must have at least one old location.");
        }
        Id = ObjectId.GenerateNewId();
        BusyRange = busyRange;
        NewLocations = newLocations;
        OldLocations = oldLocations;
        IsDone = false;
    }
}