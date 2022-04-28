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

public class SecretaryUI : ConsoleUI
{
    public SecretaryUI(Hospital _hospital, User? _user) : base(_hospital) 
    {
        this._user = _user;
    }

    public List<string> Commands {get; private set;} = new List<string> {"Options", "Help", "Exit"};
    public List<string> CRUDCommands {get; private set;} = new List<string> {"Creat", "Read", "Update", "Delete"};

    public void printCommands(List<string> commands)
    {
        Console.WriteLine("Available commands: ");
        foreach (string command in commands)
        {
            Console.WriteLine(command);
        }
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

    public override void Start()
    {
        Console.Clear();
        System.Console.WriteLine("");

        printCommands(Commands);
        while (true){
            string selectedOption = selectOption();
            if (selectedOption == "options")
            {
                CRUDoptions();
            }
            else if (selectedOption == "help")
            {
                Console.Clear();
                printCommands(Commands);
            }
            else if (selectedOption == "exit")
            {   
                Console.Clear();
                Console.WriteLine("Exiting...");
                Environment.Exit(0);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Unrecognized command, please try again");
            }
        }
    }
    public void CRUDoptions(){
        Console.Clear();
        System.Console.WriteLine("");

        printCommands(CRUDCommands);
    }
}