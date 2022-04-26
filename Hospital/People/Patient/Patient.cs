namespace Hospital
{
    public class Patient : Person
    {
        public string Sickness { get; }

        public Patient(string firstName, string lastName) : base(firstName, lastName)
        {
            Sickness = "DEATH";
        }
    }
}