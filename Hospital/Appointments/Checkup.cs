using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

using HospitalSystem.Utils;

namespace HospitalSystem
{
    public class Checkup : Appointment
    {
        [BsonIgnore]
        public static TimeSpan DefaultDuration { get; } = new TimeSpan(0,0,15,0);

        public string Anamnesis { get; set; }

        public Checkup(DateTime startTime, MongoDBRef patient, MongoDBRef doctor, string anamnesis)
            : base(new DateRange(startTime, startTime.Add(DefaultDuration)), patient, doctor)
        {
            Anamnesis = anamnesis;
        }

        [BsonConstructor]
        public Checkup(DateRange dateRange, MongoDBRef patient, MongoDBRef doctor, string anamnesis)
            : base(dateRange, patient, doctor)
        {
            Anamnesis = anamnesis;
        }

        public override string ToString()
        {
            return DateRange + " " + Patient.Id + " " + Doctor.Id + " " + DateRange.GetDuration() + " " + Anamnesis;
        }
    }
}