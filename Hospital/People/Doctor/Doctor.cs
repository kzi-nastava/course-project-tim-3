namespace Hospital
{

    public enum Specialty
    {
        DERMATOLOGY,
        RADIOLOGY,
        STOMATOLOGY,
        OPHTHALMOLOGY,
        FAMILY_MEDICINE
    }
    public class Doctor : Person
    {
        public Specialty Specialty {get; set;}
        
        public Doctor(string firstName, string lastName, Specialty specialty) : base(firstName, lastName)
        {
            Specialty = specialty;
        }

        public override string ToString (){
            return FirstName + " " + LastName;
        }

        //TODO: implement this
        public bool CheckIfFree (DateTime appointmentDate)
        {
            return true;
        }
        
    }
}