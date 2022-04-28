namespace Hospital
{
    public class Checkup : Appointment
    {
        public string Anamnesis {get; set;}

        public Checkup(DateTime timeAndDate, Patient patient, Doctor doctor, TimeSpan duration, string anamnesis) : base(timeAndDate, patient, doctor, duration)
        {
            Anamnesis = anamnesis;
        }
    }
}