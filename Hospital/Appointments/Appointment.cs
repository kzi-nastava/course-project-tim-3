using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Hospital
{
    [BsonIgnoreExtraElements]
    public class Appointment {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime TimeAndDate {get; set;}
        public MongoDBRef Patient {get; set;}
        public MongoDBRef Doctor {get; set;}
        public TimeSpan Duration {get; set;} = new TimeSpan(0,0,15,0);

        public Appointment(DateTime timeAndDate, MongoDBRef patient, MongoDBRef doctor) 
        {
            Id = ObjectId.GenerateNewId();
            TimeAndDate = timeAndDate;
            Patient = patient;
            Doctor = doctor;
        }
        public string toString()
        {
            return Id + " " + TimeAndDate + " " + Patient + " " + Doctor + " " + Duration;
        }
    
    }
}