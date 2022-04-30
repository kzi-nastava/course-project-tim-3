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
            return Id + " " + TimeAndDate + " " + Patient + " " + Doctor + " " + Duration + " " + Report;
        }
    }
}
