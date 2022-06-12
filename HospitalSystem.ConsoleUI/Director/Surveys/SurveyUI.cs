using HospitalSystem.Core;

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
            System.Console.Write("Input anything to continue >> ");
            ReadSanitizedLine();
        }
    }
}