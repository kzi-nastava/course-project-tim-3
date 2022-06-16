using System.Globalization;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core;
using HospitalSystem.Core.Surveys;


namespace HospitalSystem.ConsoleUI;
//TODO:CHANGE THIS
public class MedicalRecordUI : UserUI
{
    
    private Patient _loggedInPatient;

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
                RateDoctor(doctor, _loggedInPatient);
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

    public void ShowCheckups(AppointmentInTime checkupTime)
    {
        //unnecessary but code wouldnt compile
        List<Checkup> checkups = new List<Checkup>();
        switch (checkupTime)
        {
            case AppointmentInTime.ALL:
                checkups = _hospital.AppointmentService.GetCheckupsByPatient(_loggedInPatient.Id);
                break;   
            case AppointmentInTime.FUTURE:
                checkups = _hospital.AppointmentService.GetFutureCheckupsByPatient(_loggedInPatient.Id);
                break;
            case AppointmentInTime.PAST:
                checkups = _hospital.AppointmentService.GetPastCheckupsByPatient(_loggedInPatient.Id);
                break;
        }
        
        if (checkups.Count == 0)
        {
            Console.WriteLine("No checkups.");
            return;
        }
        for (int i = 0; i< checkups.Count; i++)
        {
            Console.WriteLine(i+" - "+ConvertAppointmentToString(checkups[i]));
        }
    }

    public void RateDoctor(Doctor doctor, Patient patient)
    {
        var surveys = _hospital.DoctorSurveyService.GetSpecificDoctorUnansweredBy(patient,doctor).ToList();
        PrintDoctorSurveys(surveys);
        if (surveys.Count == 0)
        {
                return;
        }
        System.Console.Write("Please enter a number from a list: ");
        int selectedIndex;
        try
        {
            selectedIndex = ReadInt(0, surveys.Count-1);
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            return;
        }

        CompleteSurvey(surveys[selectedIndex],doctor);
        System.Console.WriteLine("Survey completed.");
    }

    public string ConvertAppointmentToString(Appointment a)
    {
        string output = "";

        output += a.DateRange.Starts +" ";
        Doctor doctor = _hospital.DoctorService.GetById((ObjectId)a.Doctor.Id);
        output += doctor.ToString();

        return output;
    }

    public void CompleteSurvey(Survey survey,Doctor? doctor = null)
    {
        List<string?> answers = AnswerQuestions(survey);
        List<int?> ratings = AnswerRatingQuestions(survey);
        SurveyResponse response = new(answers,ratings,_loggedInPatient.Id);

        if (doctor is not null)
        {
            _hospital.DoctorSurveyService.AddResponse((DoctorSurvey)survey,response,(Doctor)doctor);
            return;
        }
        _hospital.HospitalSurveyService.AddResponse((HospitalSurvey)survey,response);
    }

     public void PrintDoctorSurveys(List<DoctorSurvey> surveys)
    {
        if (surveys.Count == 0)
        {
            System.Console.WriteLine("No unanswered surveys for selected doctor were found.");
            return;
        }
        for (int i=0; i<surveys.Count; ++i)
        {
            System.Console.WriteLine(i + " - " + surveys[i].Title);
        }
    }

    List<string?> AnswerQuestions(Survey survey)
    {
        List<string?> answers = new();
        foreach( var question in survey.Questions)
        {
            System.Console.WriteLine(question);
            string answer = ReadSanitizedLine();
            answers.Add(answer);
        }
        return answers;
    }

    List<int?> AnswerRatingQuestions(Survey survey)
    {
        List<int?> ratings = new();
        foreach( var ratingQuestion in survey.RatingQuestions)
        {
            System.Console.WriteLine(ratingQuestion);
            int rating;
            while (true)
            {
                System.Console.Write("Please enter a rating between 1 and 5: ");
                try
                {
                    rating = ReadInt(1, 5);
                    break;
                }
                catch (InvalidInputException e)
                {
                    System.Console.WriteLine(e.Message + " Please try again.");
                }
            }
            ratings.Add(rating);
        }
        return ratings;
    }

}


