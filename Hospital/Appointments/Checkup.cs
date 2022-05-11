using MongoDB.Driver;

namespace Hospital
{
    public class Checkup : Appointment
    {
        public string Anamnesis {get; set;}

        public Checkup(DateTime startTime, MongoDBRef patient, MongoDBRef doctor, string anamnesis) : base(startTime, patient, doctor)
        {
            Anamnesis = anamnesis;
        }
        public override string ToString()
        {
            return StartTime + " " + Patient.Id + " " + Doctor.Id + " " + Duration + " " + Anamnesis;
        }
    }
}