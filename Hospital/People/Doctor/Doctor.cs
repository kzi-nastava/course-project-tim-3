namespace Hospital
{
    public class Doctor : Person
    {
        public String Specialty {get; set;}
        
        public Doctor(string firstName, string lastName, string specialty) : base(firstName, lastName)
        {
            Specialty = specialty;
        }

        public string toString() 
        {
            return Id + " " +FirstName +" "+ LastName +" "+ Specialty;
        }
    }
}