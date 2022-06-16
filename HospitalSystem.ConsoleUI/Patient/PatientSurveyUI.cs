using System.Globalization;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core;
using HospitalSystem.Core.Surveys;

namespace HospitalSystem.ConsoleUI;

public class PatientSurveyUI : UserUI
{
    
    private Patient _loggedInPatient;

    public PatientSurveyUI(Hospital hospital, User user) : base(hospital, user) 
    {
        _loggedInPatient = _hospital.PatientService.GetPatientById((ObjectId) user.Person.Id);
    }

    public override void Start()
    {
       List<HospitalSurvey> surveys = _hospital.HospitalSurveyService.GetUnansweredBy(_loggedInPatient).ToList();
       PrintHospitalSurveys(surveys);
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
        CompleteSurvey(surveys[selectedIndex]);
        System.Console.WriteLine("Survey completed.");
    }
    public void PrintHospitalSurveys(List<HospitalSurvey> surveys)
    {
        if (surveys.Count == 0)
        {
            System.Console.WriteLine("No hospital surveys found.");
            return;
        }
        for (int i=0; i<surveys.Count; ++i)
        {
            System.Console.WriteLine(i + " - " + surveys[i].Title);
        }
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
}


