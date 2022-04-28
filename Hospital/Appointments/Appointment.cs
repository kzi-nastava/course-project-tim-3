using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hospital
{
    public abstract class Appointment {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime TimeAndDate {get; set;}
        public Patient Patient {get; set;}
        public Doctor Doctor {get; set;}
        public TimeSpan Duration {get; set;} = new TimeSpan(0,0,15,0);

        public Appointment(DateTime timeAndDate, Patient patient, Doctor doctor, TimeSpan duration) 
        {
            Id = ObjectId.GenerateNewId();
            TimeAndDate = timeAndDate;
            Patient = patient;
            Doctor = doctor;
            Duration = duration;
        }

        
    }
}