using HospitalSystem.Core;
using HospitalSystem.Core.Surveys;

namespace HospitalSystem.ConsoleUI.Director.Surveys;

public class DoctorSurveyUI : SurveyUI
{
    private List<DoctorSurvey> _loadedSurveys;

    public DoctorSurveyUI(Hospital hospital) : base(hospital)
    {
        _loadedSurveys = new();
    }

    private void RefreshSurveys()
    {
        _loadedSurveys = _hospital.DoctorSurveyService.GetAll().ToList();
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
        System.Console.Write("Input survey number >> ");
        var survey = _loadedSurveys[ReadInt(0, _loadedSurveys.Count-1)];
        System.Console.Clear();
        DisplayBest(survey);
        System.Console.WriteLine();
        DisplayWorst(survey);
        System.Console.Write("\nWould you like to pick any doctor to see his results? [y/N] >> ");
        if (!ReadYes())
        {
            System.Console.WriteLine("Returning to menu...");
            return;
        }
        var drResponses = ChooseDoctorResponses(survey);
        DisplayDoctorResponses(survey, drResponses.Item1, drResponses.Item2);
    }

    private void DisplayDoctorResponses(DoctorSurvey survey, Doctor dr, IList<SurveyResponse> responses)
    {
        System.Console.Clear();
        System.Console.WriteLine("Showing survey: " + survey.Title);
        System.Console.WriteLine("For doctor: " + dr.ToString());
        DisplayAggregatedRatings(survey.AggregateRatingsFor(dr));
        DisplayResponses(survey.Questions, responses);
    }

    private (Doctor, List<SurveyResponse>) ChooseDoctorResponses(DoctorSurvey survey)
    {
        var allDrResponses = _hospital.DoctorSurveyService.GetDoctorsWithResponsesFor(survey);
        DisplayDoctors(allDrResponses);
        System.Console.Write("Input doctor number >> ");
        return allDrResponses[ReadInt(0, allDrResponses.Count - 1)];
    }

    private void DisplayBest(DoctorSurvey survey)
    {
        System.Console.WriteLine("--- Best Doctors ---");
        DisplayRatedDoctors(_hospital.DoctorSurveyService.GetBestDoctors(survey));
    }

    private void DisplayWorst(DoctorSurvey survey)
    {
        System.Console.WriteLine("--- Worst Doctors ---");
        DisplayRatedDoctors(_hospital.DoctorSurveyService.GetWorstDoctors(survey));
    }

    private void DisplayRatedDoctors(IList<RatedDoctor> drRatings)
    {
        System.Console.WriteLine("No. | Doctor | Avg rating | Rating count");
        for (int i = 0; i < drRatings.Count; i++)
        {
            System.Console.WriteLine(i+1 + " | " + drRatings[i].Doctor.ToString() +
                " | " + (drRatings[i].Average?.ToString() ?? " / ") + " | " + drRatings[i].Count);
        }
    }

    // TODO: there might be something that does this in PatUI
    private void DisplayDoctors(IList<(Doctor, List<SurveyResponse>)> drResponses)
    {
        System.Console.WriteLine("No. | Doctor");
        for (int i = 0; i < drResponses.Count; i++)
        {
            System.Console.WriteLine(i + " | " + drResponses[i].Item1.ToString());
        }
    }
}