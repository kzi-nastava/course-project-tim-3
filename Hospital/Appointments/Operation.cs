using MongoDB.Driver;

namespace HospitalSystem
{
    public class Operation : Appointment
    {
       public string Report {get; set;}
       public Operation(DateTime startTime, MongoDBRef patient, MongoDBRef doctor, string report, TimeSpan duration) : base(startTime, patient, doctor)
       {
           Duration = duration;
           Report = report;
       }
       public override string ToString()
        {
            return StartTime + " " + Patient.Id + " " + Doctor.Id + " " + Duration + " " + Report;
        }
    }
}
