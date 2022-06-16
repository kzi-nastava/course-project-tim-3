using MongoDB.Bson;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

public class NotificationUI : PatientUI
{
    public NotificationUI(Hospital hospital, User user) : base(hospital, user) 
    {
        _loggedInPatient = _hospital.PatientService.GetById((ObjectId) user.Person.Id);
    }

    public override void Start()
    {
        System.Console.WriteLine(@"
            Options:
            s - show notifications
            w - set when to remind
            ");
        string sortOption = ReadSanitizedLine().Trim();
        if (sortOption == "s")
        {
            ShowNotifications();
        }
        else if (sortOption == "w")
        {
            SetNotificationSettings();
        }
    }
    
    public void ShowNotifications()
    {
        int notificationCount = 0;
        foreach (Prescription prescription in _loggedInPatient.MedicalRecord.Prescriptions)
        {
            DateTime ?whenToTake = _hospital.PatientService.WhenToTakeMedicine(prescription,_loggedInPatient);
            if (whenToTake is not null)
            {
                notificationCount += 1;
                string meal = "";
                switch(prescription.BestTaken)
                {
                    case MedicationBestTaken.AFTER_MEAL:
                        meal = "after meal";
                        break;
                    case MedicationBestTaken.BEFORE_MEAL:
                        meal = "before meal";
                        break;
                    case MedicationBestTaken.ANY_TIME:
                        meal = "any time";
                        break;
                    case MedicationBestTaken.WITH_MEAL:
                        meal = "with meal";
                        break;
                }
                Console.WriteLine("Take "+prescription.Medication.Name+" at "+ whenToTake?.ToString("HH:mm")+" best taken "+meal);
            }
        }
        if (notificationCount==0)
        {
            Console.WriteLine("No notifications.");
        }
    }
    public void SetNotificationSettings()
    {
        int numberOfMinutes;
        Console.WriteLine("Please enter how many minutes before perscription should be considered (min 5, max 300): ");
        try
        {
            numberOfMinutes = ReadInt(5, 300);
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            throw new QuitToMainMenuException("Wrong input");
        }
        _loggedInPatient.WhenToRemind = TimeSpan.FromMinutes(numberOfMinutes);
        _hospital.PatientService.Upsert(_loggedInPatient);
        Console.WriteLine("Preference saved.");
    }
}


