using HospitalSystem.Core;
using HospitalSystem.Core.Surveys;

namespace HospitalSystem.ConsoleUI.Director.Surveys;

public class HospitalSurveyUI : SurveyUI
{
    private List<HospitalSurvey> _loadedSurveys;

    public HospitalSurveyUI(Hospital hospital) : base(hospital)
    {
        _loadedSurveys = new();
    }

    private void RefreshSurveys()
    {
        _loadedSurveys = _hospital.SurveyService.GetAllHospital().ToList();
    }

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("--- AVAILABLE SURVEYS ---");
            RefreshSurveys();
            DisplaySurveys(_loadedSurveys.Cast<Survey>().ToList());
            System.Console.WriteLine(@"
            INPUT OPTION:
                [view|v] View survey
                [quit|q] Quit to main menu
                [exit|x] Exit program
            ");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                if (choice == "v" || choice == "view")
                {
                    DisplaySurvey();
                }
                else if (choice == "q" || choice == "quit")
                {
                    throw new QuitToMainMenuException("From StartManageMedicationRequests.");
                }
                else if (choice == "x" || choice == "exit")
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    System.Console.WriteLine("Invalid input - please read the available commands.");
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.WriteLine(e.Message);
            }
            System.Console.Write("Input anything to continue >> ");
            ReadSanitizedLine();
        }
    }

    private void DisplaySurvey()
    {
        System.Console.Write("Input number >> ");
        var survey = _loadedSurveys[ReadInt(0, _loadedSurveys.Count-1)];
        System.Console.Clear();
        System.Console.WriteLine("Showing survey: " + survey.Title);
        System.Console.WriteLine("Ratings:");
        DisplayAggregatedRatings(survey);
        DisplayResponses(survey.Questions, survey.Responses);
    }

    private void DisplayAggregatedRatings(HospitalSurvey survey)
    {
        DisplayAggregatedRatings(survey.AggregateRatings());
    }
}