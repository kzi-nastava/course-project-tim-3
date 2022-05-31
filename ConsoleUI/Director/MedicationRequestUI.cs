namespace HospitalSystem.ConsoleUI;

public class MedicationRequestUI : ConsoleUI
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
                    System.Console.Write("Input anything to continue >> ");
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Input anything to continue >> ");
            }
            ReadSanitizedLine();
        }
    }

    public void DisplayRequests()  // assumes DENIED status, so no need to show status field
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

    public void CreateRequest()
    {
        // TODO: extract methods
        System.Console.Write("Enter Medication name >> ");
        var name = ReadSanitizedLine();
        if (name == "")
        {
            throw new InvalidInputException("Name can not be empty.");
        }

        System.Console.WriteLine("Input ingredients. Each line is one ingredients. " + 
            "Empty line means that you are done with inputting ingredients");
        List<string> ingredients = new();
        var ingredient = "INVALID";
        while (ingredient != "")
        {
            System.Console.Write(">> ");
            ingredient = ReadSanitizedLine();
            if (ingredient != "")
            {
                ingredients.Add(ingredient);
            }
        }

        System.Console.Write("Input your comment >> ");
        var comment = ReadSanitizedLine();
        if (comment == "")
        {
            comment = "/";
        }
        var req = new MedicationRequest(new Medication(name, ingredients), comment);
        _hospital.MedicationRequestService.Send(req);
    }
}