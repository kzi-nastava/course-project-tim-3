namespace Hospital
{
    public class Patient : Person
    {
        public MedicalRecord MedicalRecord {get; set;}
        public Patient(string firstName, string lastName, MedicalRecord medicalRecord) : base(firstName, lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            MedicalRecord = medicalRecord;
        }

        public override string ToString()
        {
            return FirstName + " " + LastName ;
        }
    }
}