namespace Hospital;

[System.Serializable]
public class NullInputException : System.Exception
{
    public NullInputException() { }
    public NullInputException(string message) : base(message) { }
    public NullInputException(string message, System.Exception inner) : base(message, inner) { }
    protected NullInputException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
public class PatientUI : ConsoleUI
{
    public PatientUI(Hospital _hospital, User? _user) : base(_hospital) 
    {
        this._user = _user;
        
    }

    public string selectOption()
    {
        
        Console.Write("Please enter a command: ");
        string? input = Console.ReadLine();
        if (input is null)
        {
            throw new NullInputException("Null value as input");
        }
        return input.ToLower().Trim();

    }

    public List<string> Commands {get; private set;} = new List<string> {"ma / manage appointments","exit","help"};

    public void manageAppointments()
    {
        Console.WriteLine("wip");
    }

    public void printCommands()
    {
        Console.WriteLine("Available commands: ");
        foreach (string command in this.Commands)
        {
            Console.WriteLine(command);
        }
    }

    public override void Start()
    {
        Console.Clear();
        System.Console.WriteLine("");

        printCommands();
        while (true){
            string selectedOption = selectOption();
            if (selectedOption == "ma" || selectedOption == "manage appointments")
            {
                manageAppointments();
            }
            else if (selectedOption == "help")
            {
                printCommands();
            }
            else if (selectedOption == "exit")
            {
                Console.WriteLine("Exiting...");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Unrecognized command, please try again");
            }
        }
    }
}