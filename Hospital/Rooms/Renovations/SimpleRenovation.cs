using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Hospital;

public class SimpleRenovation
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateTime WhenDone { get; set; }
    public string RoomLocation { get; set; }
    public bool IsDone { get; set; }

    public SimpleRenovation(string roomLocation, DateTime whenDone)
    {
        Id = ObjectId.GenerateNewId();
        RoomLocation = roomLocation;
        WhenDone = whenDone;
        IsDone = false;
    }
}