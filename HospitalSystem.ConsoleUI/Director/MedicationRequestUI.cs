using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

public class MedicationRequestUI : HospitalClientUI
{
    private List<MedicationRequest> _loadedRequests = new();

    public MedicationRequestUI(Hospital hospital) : base(hospital)
    {

    }

    private void RefreshRequests()
    {
        _loadedRequests = _hospital.MedicationRequestService.GetDenied().ToList();
    }

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("--- DENIED REQUESTS ---");
            RefreshRequests();
            DisplayRequests();
            System.Console.WriteLine(@"
            INPUT OPTION:
                [create request|cr] Create a request for new medication
                [edit request|er] Edit a denied request
                [quit|q] Quit to main menu
                [exit|x] Exit program
            ");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                if (choice == "cr" || choice == "create request")
                {
                    CreateRequest();
                }
                else if (choice == "er" || choice == "edit request")
                {
                    EditRequest();
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

    private void DisplayRequests()  // assumes DENIED status, so no need to show status field
    {
        System.Console.WriteLine("No. | Doctors Comment | Directors Comment | "
            + "Medication Name | Medication Ingredients");
        // TODO: paginate and make prettier
        for (int i = 0; i < _loadedRequests.Count; i++)
        {
            var req = _loadedRequests[i];
            System.Console.WriteLine(i + " | " + req.DoctorComment + " | " + req.DirectorComment
                + " | " + req.Requested.Name + " | " + String.Join(", ", req.Requested.Ingredients));
        }
    }

    private void CreateRequest()
    {
        System.Console.Write("Enter Medication name >> ");
        var name = ReadSanitizedLine();
        if (name == "")
        {
            throw new InvalidInputException("Name can not be empty.");
        }

        List<string> ingredients = new();
        var ingredientUI = new IngredientsUI(ingredients);
        ingredientUI.Start();
        System.Console.Write("Input your comment >> ");
        var comment = ReadSanitizedLine();
        if (comment == "")
        {
            comment = "/";
        }
        var req = new MedicationRequest(new Medication(name, ingredients), comment);
        _hospital.MedicationRequestService.Send(req);
        System.Console.Write("Success! ");
    }

    private void EditRequest()
    {
        System.Console.Write("Input request number >> ");
        var num = ReadInt(0, _loadedRequests.Count - 1);
        var req = _loadedRequests[num];
        System.Console.WriteLine("Leave line blank for no changes.");

        System.Console.Write("Enter Medication name >> ");
        var name = ReadSanitizedLine();
        if (name != "")
        {
            req.Requested.Name = name;
        }

        System.Console.Write("Edit ingredients? [y/N] >> ");
        var choice = ReadSanitizedLine();
        if (choice == "y")
        {
            var ingredientUI = new IngredientsUI(req.Requested.Ingredients);
            ingredientUI.Start();
        }

        System.Console.Write("Input your comment >> ");
        var comment = ReadSanitizedLine();
        if (comment != "")
        {
            req.DirectorComment = comment;
        }
        _hospital.MedicationRequestService.Resend(req);
        System.Console.Write("Success! ");
    }

}