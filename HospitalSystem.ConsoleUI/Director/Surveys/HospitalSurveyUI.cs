using HospitalSystem.Core;
using HospitalSystem.Core.Surveys;

namespace HospitalSystem.ConsoleUI.Director.Surveys;

public class HospitalSurveyUI : HospitalClientUI
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
        System.Console.WriteLine("Showing survey: " + survey.Title);
        System.Console.WriteLine("Ratings:");
        DisplayAggregatedRatings(survey);
        DisplayAnswers(survey);
    }

    private void DisplayAggregatedRatings(HospitalSurvey survey)
    {
        var aggregatedRatings = survey.AggregateRatings();
        foreach (var aggregate in aggregatedRatings)
        {
            System.Console.WriteLine(aggregate.Item1);
            System.Console.WriteLine("Average: " + (aggregate.Item2?.ToString() ?? "/") + ", Count: " + aggregate.Item3);
            System.Console.WriteLine();
        }
    }

    private void DisplayAnswers(HospitalSurvey survey)
    {
        int ansCount = 0;
        for (int i = 0; i < survey.Answers.Count; i++)
        {
            if (survey.Answers[i].Answers.Any(ans => ans != null))
            {
                DisplayAnswer(ansCount, survey.Questions, survey.Answers[i].Answers);
                ansCount++;
            }
        }
    }

    private void DisplayAnswer(int num, List<string> questions, List<string?> answers)
    {
        System.Console.WriteLine("Answer #" + num);
        for (int j = 0; j < questions.Count; j++)
        {
            if (answers[j] != null)
            {
                System.Console.WriteLine(questions[j]);
                System.Console.Write("Answer: ");
                System.Console.WriteLine(answers[j]);  // TODO: rename answer to response
                System.Console.WriteLine();
            }
        }
        System.Console.WriteLine();
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