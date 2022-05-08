using MongoDB.Bson.Serialization.Attributes;
namespace Hospital
{
    public class Patient : Person
    {
        [BsonElement]
        public MedicalRecord MedicalRecord {get; set;}
        public List<CheckupChangeLog> CheckupChangeLogs {get; set;}
        public Patient(string firstName, string lastName, MedicalRecord medicalRecord) : base(firstName, lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            MedicalRecord = medicalRecord;
            CheckupChangeLogs = new List<CheckupChangeLog>();
        }

        public Patient(string firstName, string lastName, MedicalRecord medicalRecord, List<CheckupChangeLog> checkupChangeLogs) : base(firstName, lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            MedicalRecord = medicalRecord;
            CheckupChangeLogs = checkupChangeLogs;
        }

        public override string ToString()
        {
            return FirstName + " " + LastName ;
        }
    }
}