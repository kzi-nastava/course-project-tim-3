using MongoDB.Bson.Serialization.Attributes;
namespace HospitalSystem.Core
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

        public override string ToString()
        {
            return FirstName + " " + LastName ;
        }

        public bool IsAllergicToMedication(Medication medication)
        {
            foreach (string ingredient in medication.Ingredients)
            {
                if (MedicalRecord.Allergies.Contains(ingredient.ToLower())) return true;
            }
            return false;
        }
    }
}