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
        public Medicine Medicine {get; set;}
        public int TimesADay {get; set;}
        public MedicineBestTaken BestTaken {get; set;}
        public int HoursBetweenIntakes {get; set;}

        public Prescription(Medicine medicine, int timesADay, MedicineBestTaken bestTaken, int hoursbetweenIntakes)
        {
            Medicine = medicine;
            TimesADay = timesADay;
            BestTaken = bestTaken;
            HoursBetweenIntakes = hoursbetweenIntakes;
        }
    }
}