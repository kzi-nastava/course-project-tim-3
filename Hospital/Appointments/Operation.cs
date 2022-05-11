using MongoDB.Driver;

namespace Hospital
{
    public class Operation : Appointment
    {
       public string Report {get; set;}
       public Operation(DateTime startTime, MongoDBRef patient, MongoDBRef doctor, string report) : base(startTime, patient, doctor)
       {
           Report = report;
       }
       public override string ToString()
        {
            return StartTime + " " + Patient.Id + " " + Doctor.Id + " " + Duration + " " + Report;
        }
    }
}
