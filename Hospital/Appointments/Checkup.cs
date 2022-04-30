using MongoDB.Driver;

namespace Hospital
{
    public class Checkup : Appointment
    {
        public string Anamnesis {get; set;}

        public Checkup(DateTime timeAndDate, MongoDBRef patient, MongoDBRef doctor, string anamnesis) : base(timeAndDate, patient, doctor)
        {
            Anamnesis = anamnesis;
        }
        public override string ToString()
        {
            return TimeAndDate + " " + Patient.Id + " " + Doctor.Id + " " + Duration + " " + Anamnesis;
        }
    }
}