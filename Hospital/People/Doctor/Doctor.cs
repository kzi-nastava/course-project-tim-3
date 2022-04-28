namespace Hospital
{
    public class Doctor : Person
    {
        private String Specialty {get;}
        public Doctor(string firstName, string lastName, string specialty) : base(firstName, lastName)
        {
            Specialty = specialty;
        }
    }
}