using HospitalSystem.Core;
using HospitalSystem.Core.Surveys;

namespace HospitalSystem.ConsoleUI.Director.Surveys;

public class SurveyUI : HospitalClientUI
{
    public SurveyUI(Hospital hospital) : base(hospital)
    {
        
    }

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine(@"
            INPUT OPTION:
                [hospital|h] View hospital surveys
                [doctor|d] View doctor surveys
                [quit|q] Quit to main menu
                [exit|x] Exit program
            ");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                if (choice == "h" || choice == "hospital")
                {
                    var hospitalSurveyUI = new HospitalSurveyUI(_hospital);
                    hospitalSurveyUI.Start();
                }
                else if (choice == "d" || choice == "doctor")
                {
                    var doctorSurveyUI = new DoctorSurveyUI(_hospital);
                    doctorSurveyUI.Start();
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
            catch (InvalidSurveyException e)
            {
                System.Console.WriteLine(e.Message);
            }
            System.Console.Write("Input anything to continue >> ");
            ReadSanitizedLine();
        }
    }

    protected void DisplayResponses(IList<string> questions, IEnumerable<SurveyResponse> responses)
    {
        int ansCount = 0;
        foreach (var response in responses)
        {
            if (response.Answers.Any(ans => ans != null))
            {
                DisplayResponse(ansCount, questions, response.Answers);
                ansCount++;
            }
        }
    }

    protected void DisplayResponse(int num, IList<string> questions, IList<string?> answers)
    {
        System.Console.WriteLine("Response #" + (num + 1));
        for (int j = 0; j < questions.Count; j++)
        {
            if (answers[j] != null)
            {
                System.Console.WriteLine(questions[j]);
                System.Console.Write("Answer: ");
                System.Console.WriteLine(answers[j]);
                System.Console.WriteLine();
            }
        }
        System.Console.WriteLine();
    }

    protected void DisplaySurveys(IList<Survey> surveys)
    {
        System.Console.WriteLine("No. | Title");
        for (int i = 0; i < surveys.Count; i++)
        {
            System.Console.WriteLine(i + " | " + surveys[i].Title);
        }
    }

    protected void DisplayAggregatedRatings(IEnumerable<AggregatedRating> aggregatedRatings)
    {
        foreach (var aggregate in aggregatedRatings)
        {
            System.Console.WriteLine(aggregate.Question);
            System.Console.WriteLine("Average: " + (aggregate.Average?.ToString() ?? "/") +
                ", Count: " + aggregate.Count);
            System.Console.WriteLine();
        }
    }
}