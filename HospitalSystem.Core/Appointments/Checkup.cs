using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core
{
    public class Checkup : Appointment
    {
        [BsonIgnore]
        public static TimeSpan DefaultDuration { get; } = new TimeSpan(0,0,15,0);

        public string Anamnesis { get; set; }

        public DoctorSurvey ?DoctorSurvey { get; set; }

        public Checkup(DateTime startTime, MongoDBRef patient, MongoDBRef doctor, string anamnesis, DoctorSurvey ?doctorSurvey = null)
            : base(new DateRange(startTime, startTime.Add(DefaultDuration), false), patient, doctor)
        {
            Anamnesis = anamnesis;
            DoctorSurvey = doctorSurvey;
        }

        [BsonConstructor]
        public Checkup(DateRange dateRange, MongoDBRef patient, MongoDBRef doctor, string anamnesis, DoctorSurvey ?doctorSurvey = null)
            : base(dateRange, patient, doctor)
        {
            Anamnesis = anamnesis;
            DoctorSurvey = doctorSurvey;
        }

        public override string ToString()
        {   
            string survey = "none";
            if (DoctorSurvey is not null)
            {
                survey = DoctorSurvey.ToString();
            }
            return DateRange + " " + Patient.Id + " " + Doctor.Id + " " + DateRange.GetDuration() + " " + Anamnesis + survey;
        }
    }
}