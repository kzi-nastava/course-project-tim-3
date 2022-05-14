namespace Hospital
{
    public enum MedicineBestTaken
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
        public MedicineBestTaken BestTaken {get; set;}
        public int HoursBetweenIntakes {get; set;}

        public Prescription(Medication medication, int timesADay, MedicineBestTaken bestTaken, int hoursbetweenIntakes)
        {
            Medication = medication;
            TimesADay = timesADay;
            BestTaken = bestTaken;
            HoursBetweenIntakes = hoursbetweenIntakes;
        }
    }
}