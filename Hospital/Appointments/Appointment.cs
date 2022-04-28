using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hospital
{
    public abstract class Appointment {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime TimeAndDate {get; set;}
        public Patient _Patient {get; set;}
        public Doctor _Doctor {get; set;}
        public int Duration {get; set;}

        public Appointment(DateTime timeAndDate, Patient patient, Doctor doctor, int duration) 
        {
            Id = ObjectId.GenerateNewId();
            TimeAndDate = timeAndDate;
            _Patient = patient;
            _Doctor = doctor;
            Duration = duration;
        }
    }
}