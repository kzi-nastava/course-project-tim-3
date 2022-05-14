namespace Hospital
{
    public enum MedicationBestTaken
    {
        BEFORE_MEAL,
        AFTER_MEAL,
        WITH_MEAL,
        ANY_TIME
    }
    public class Prescription
    {
        public Medication Medication {get; set;}
        public int TimesADay {get; set;}
        public MedicationBestTaken BestTaken {get; set;}
        public int HoursBetweenIntakes {get; set;}

        public Prescription(Medication medication, int timesADay, MedicationBestTaken bestTaken, int hoursbetweenIntakes)
        {
            Medication = medication;
            TimesADay = timesADay;
            BestTaken = bestTaken;
            HoursBetweenIntakes = hoursbetweenIntakes;
        }
    }
}