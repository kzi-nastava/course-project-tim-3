using MongoDB.Driver;

namespace Hospital
{
    public class Referral
    {
        public MongoDBRef Patient {get; set;}
        public MongoDBRef Doctor {get; set;}
        public Referral(MongoDBRef patient, MongoDBRef doctor)
        {
            Patient = patient;
            Doctor = doctor;
        }

        public override string ToString()
    {
        return Patient.Id + " " + Doctor.Id;
    }
    }

    
}