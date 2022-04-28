namespace Hospital
{
    public class Doctor : Person
    {
        private String Specialty {get; set;}
        
        public Doctor(string firstName, string lastName, string specialty) : base(firstName, lastName)
        {
            Specialty = specialty;
        }
    }
}