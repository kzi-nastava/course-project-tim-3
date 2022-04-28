namespace Hospital
{
    public class Checkup : Appointment
    {
        public string Anamnesis {get; set;}

        public Checkup(DateTime timeAndDate, Patient patient, Doctor doctor, int duration, string anamnesis) : base(timeAndDate, patient, doctor, duration)
        {
            Anamnesis = anamnesis;
        }

    }
}