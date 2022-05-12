using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Hospital;

public class SimpleRenovation
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateTime EndTime { get; set; }
    public string RoomLocation { get; set; }
    public bool IsDone { get; set; }

    public SimpleRenovation(string roomLocation, DateTime endTime)
    {
        Id = ObjectId.GenerateNewId();
        RoomLocation = roomLocation;
        EndTime = endTime;
        IsDone = false;
    }
}