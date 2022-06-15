using MongoDB.Driver;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

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