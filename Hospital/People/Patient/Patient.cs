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

        public override string ToString()
        {
            return FirstName + " " + LastName ;
        }

        public bool IsAllergicToMedicine(Medicine medicine)
        {
            foreach (string ingredient in medicine.Ingredients)
            {
                if (MedicalRecord.Allergies.Contains(ingredient)) return true;
            }
            return false;
        }
    }
}