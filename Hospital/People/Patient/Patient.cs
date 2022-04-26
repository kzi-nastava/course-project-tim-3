namespace Hospital
{
    public class Patient : Person
    {
        public string Sickness { get; }  // TODO: change this, this is temporary

        public Patient(string firstName, string lastName, string sickness = "DEATH") : base(firstName, lastName)
        {
            Sickness = sickness;
        }
    }
}