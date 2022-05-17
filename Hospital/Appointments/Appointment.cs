using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Hospital
{
    [BsonIgnoreExtraElements]
    public class Appointment {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime StartTime {get; set;}

        public MongoDBRef Patient {get; set;}
        public MongoDBRef Doctor {get; set;}
        public TimeSpan Duration {get; set;} = new TimeSpan(0,0,15,0);
        public DateTime EndTime {get; set;}
        public string? RoomLocation { get; set; }

        public Appointment(DateTime startTime, MongoDBRef patient, MongoDBRef doctor) 
        {
            Id = ObjectId.GenerateNewId();
            StartTime = startTime;
            Patient = patient;
            Doctor = doctor;
            EndTime = startTime.Add(Duration);
            RoomLocation = null;
        }

        public override string ToString()
        {
            return StartTime + " " + Patient.Id + " " + Doctor.Id + " " + Duration;
        }
    
    }
}