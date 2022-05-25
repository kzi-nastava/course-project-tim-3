using MongoDB.Driver;
using HospitalSystem.Utils;

namespace HospitalSystem;

public class Operation : Appointment
{
    public string Report { get; set; }

    public Operation(DateRange range, MongoDBRef patient, MongoDBRef doctor, string report)
        : base(range, patient, doctor)
    {
        Report = report;
    }

    public override string ToString()
    {
        return DateRange + " " + Patient.Id + " " + Doctor.Id + " " + DateRange.GetDuration() + " " + Report;
    }
}