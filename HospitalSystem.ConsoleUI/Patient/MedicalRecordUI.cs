using MongoDB.Bson;
using HospitalSystem.Core;
using HospitalSystem.Core.Surveys;


namespace HospitalSystem.ConsoleUI;
public class MedicalRecordUI : PatientUI
{
    public MedicalRecordUI(Hospital hospital, User user) : base(hospital, user) 
    {
        _loggedInPatient = _hospital.PatientService.GetPatientById((ObjectId) user.Person.Id);
    }

    public override void Start()
    {
        while (true)
        {
            System.Console.WriteLine(@"
            Commands:
            sc - show past checkups
            as - anamnesis search
            return - go to the previous menu
            exit - quit the program
            ");

            string selectedOption = ReadSanitizedLine().Trim();

            try
            {
                if (selectedOption == "sc")
                {
                    StartPastCheckups();
                }
                else if (selectedOption == "as")
                {
                    StartAnamnesisSearch();
                }
                else if (selectedOption == "return")
                {
                    Console.WriteLine("Returning...\n");
                    break;
                }
                else if (selectedOption == "exit")
                {
                    Console.WriteLine("Exiting...\n");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Unrecognized command, please try again");
                }
            }
            //this might create problems
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message);
            }
        }
    }

    public void StartAnamnesisSearch()
    {
        Console.Write("Please enter a search keyword: ");
        string keyword = ReadSanitizedLine().Trim();

        List<Checkup> filteredDoctors = _hospital.AppointmentService.SearchPastCheckups(_loggedInPatient.Id,keyword);

        if (filteredDoctors.Count == 0)
        {
            Console.WriteLine("No anamnesis found");
            return;
        }

        System.Console.WriteLine(@"
            Sort options:
            d - sort by date
            n - sort by doctors name
            s - sort by specialty
            ");
        
        //there is probably a better way to do n and s, but idk
        string sortOption = ReadSanitizedLine().Trim();
        if (sortOption == "d")
        {
            filteredDoctors.Sort((checkup1, checkup2)=> 
                DateTime.Compare(checkup1.DateRange.Starts, checkup2.DateRange.Ends));
        }
        else if (sortOption == "n")
        {
            filteredDoctors.Sort(_hospital.AppointmentService.CompareCheckupsByDoctorsName);
        }
        else if (sortOption == "s")
        {
            filteredDoctors.Sort(_hospital.AppointmentService.CompareCheckupsByDoctorsSpecialty);
        }

        foreach (Checkup checkup in filteredDoctors)
        {
           ShowCheckupsAnamnesis(checkup);
        }
    }

    public void ShowCheckupsAnamnesis(Checkup checkup)
    {
        Doctor doctor = _hospital.DoctorService.GetById( (ObjectId)checkup.Doctor.Id );
        Console.WriteLine("[ " + checkup.DateRange.Starts + " " + doctor + " ] ");
        Console.WriteLine(checkup.Anamnesis);
        Console.WriteLine();
    }

    public void StartPastCheckups()
    {
        ShowCheckups(AppointmentInTime.PAST);
        List<Checkup> pastCheckups = _hospital.AppointmentService.GetPastCheckupsByPatient(_loggedInPatient.Id);
        if (pastCheckups.Count == 0)
        {
            System.Console.WriteLine("No checkups found.");
            return;
        }

        int selectedIndex;
        try
        {
            System.Console.Write("Please enter a number from the list: ");
            selectedIndex = ReadInt(0, pastCheckups.Count-1);
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            throw new QuitToMainMenuException("Wrong input");
        }
        Checkup selectedCheckup = pastCheckups[selectedIndex];

        System.Console.WriteLine(@"
        Commands:
        a - show anamnesis
        r - rate doctor
        return - go to the previous menu
        ");

        string selectedOption = ReadSanitizedLine().Trim();

        try
        {
            if (selectedOption == "a")
            {
                Console.WriteLine("Anamnesis: "+ selectedCheckup.Anamnesis);
            }
            else if (selectedOption == "r")
            {
                Doctor doctor = _hospital.DoctorService.GetById((ObjectId)selectedCheckup.Doctor.Id);
                PatientSurveyUI surveyUI = new(_hospital, _user);
                surveyUI.RateDoctor(doctor, _loggedInPatient);
                
            }
            else if (selectedOption == "return")
            {
                Console.WriteLine("Returning...\n");
                return;
            }
            else
            {
                Console.WriteLine("Unrecognized command, please try again");
            }
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message);
            return;
        }
    }
}
