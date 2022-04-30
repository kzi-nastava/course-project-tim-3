using MongoDB.Driver;

namespace Hospital
{
    public class Operation : Appointment
    {
       public string Report {get; set;}
       public Operation(DateTime timeAndDate, MongoDBRef patient, MongoDBRef doctor, string report) : base(timeAndDate, patient, doctor)
       {
           Report = report;
       }
       public override string ToString()
        {
            return TimeAndDate + " " + Patient.Id + " " + Doctor.Id + " " + Duration + " " + Report;
        }
    }
}
