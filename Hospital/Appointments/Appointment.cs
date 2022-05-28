using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using HospitalSystem.Utils;

namespace HospitalSystem;

[BsonIgnoreExtraElements]
public class Appointment
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateRange DateRange { get; set; }
    public MongoDBRef Patient { get; set; }
    public MongoDBRef Doctor { get; set; }
    public string? RoomLocation { get; set; }

    public Appointment(DateRange range, MongoDBRef patient, MongoDBRef doctor) 
    {
        Id = ObjectId.GenerateNewId();
        Patient = patient;
        Doctor = doctor;
        DateRange = range;
        RoomLocation = null;
    }
}