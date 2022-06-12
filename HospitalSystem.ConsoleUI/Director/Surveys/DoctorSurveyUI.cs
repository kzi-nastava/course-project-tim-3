using HospitalSystem.Core;
using HospitalSystem.Core.Surveys;

namespace HospitalSystem.ConsoleUI.Director.Surveys;

public class DoctorSurveyUI : HospitalClientUI
{
    private List<DoctorSurvey> _loadedSurveys;

    public DoctorSurveyUI(Hospital hospital) : base(hospital)
    {
        _loadedSurveys = new();
    }

    private void RefreshSurveys()
    {
        _loadedSurveys = _hospital.SurveyService.GetAllDoctor().ToList();
    }

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("--- AVAILABLE SURVEYS ---");
            RefreshSurveys();
            DisplaySurveys();
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
        var groupedSurveys = _hospital.SurveyService.GetResponsesGroupedByDoctor(survey);
        DisplayGroups(groupedSurveys);
        DisplayResponses(groupedSurveys[ReadInt(0, groupedSurveys.Count - 1)].Item2);
    }

    private void DisplayGroups(IList<(Doctor, IEnumerable<DoctorSurveyResponse>)> groupedSurveys)
    {
        System.Console.WriteLine("No. | Doctor");
        for (int i = 0; i < groupedSurveys.Count; i++)
        {
            System.Console.WriteLine(i + " | " + groupedSurveys[i].Item1.ToString());
        }
    }

    private void DisplayResponses(IEnumerable<DoctorSurveyResponse> responses)
    {
        // TODO: refactor so it is Survey -> DoctorId -> responses_list
    }

    private void DisplaySurveys()
    {
        System.Console.WriteLine("No. | Title");
        for (int i = 0; i < _loadedSurveys.Count; i++)
        {
            System.Console.WriteLine(i + " | " + _loadedSurveys[i].Title);
        }
    }
}