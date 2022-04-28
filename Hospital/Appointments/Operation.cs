namespace Hospital
{
    public class Operation : Appointment
    {
       public string Report {get; set;}

       public Operation(DateTime timeAndDate, Patient patient, Doctor doctor, TimeSpan duration, string report) : base(timeAndDate, patient, doctor, duration)
       {
           Report = report;
       }
    }
}
