using MongoDB.Bson.Serialization.Attributes;
namespace HospitalSystem.Core
{
    public class Patient : Person
    {
        [BsonElement]
        public MedicalRecord MedicalRecord {get; set;}
        public List<CheckupChangeLog> CheckupChangeLogs {get; set;}
        public TimeSpan WhenToRemind {get; set;}
        public Patient(string firstName, string lastName, MedicalRecord medicalRecord, TimeSpan? whenToRemind = null) : base(firstName, lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            MedicalRecord = medicalRecord;
            CheckupChangeLogs = new List<CheckupChangeLog>();

            if (whenToRemind is null){
                WhenToRemind = TimeSpan.FromHours(1);
            }
            else{
                WhenToRemind = (TimeSpan)whenToRemind;
            }
            
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