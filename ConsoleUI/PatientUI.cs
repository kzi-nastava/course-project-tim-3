namespace Hospital;
using System.Globalization;

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
    public List<string> MainCommands {get; private set;} = new List<string> {"ma / manage appointments","exit","help"};
    public List<string> ManageAppointmentsCommands {get; private set;} = new List<string> 
    {"ca / create appointment",
    "ua / update appointment",
    "va / view appointments",
    "da / delete appointment",
    "return",
    "exit",
    "help"};

    public PatientUI(Hospital _hospital, User? _user) : base(_hospital) 
    {
        this._user = _user;
    }

    DateTime? inputDate ()
    {
        Console.Write("Please enter a date in dd-MM-yyyy format: ");
        string? date = Console.ReadLine();
        if (date is null)
        {
            throw new NullInputException("Null value as input");
        }

        date = date.Trim();

        Console.Write("Please enter a time in HH:mm format :");
        string? time = Console.ReadLine();
        if (time is null)
        {
            throw new NullInputException("Null value as input");
        }

        time = time.Trim();
        DateTime selectedDateTime;
        
        bool success = DateTime.TryParseExact(date+" "+time, 
                       "dd-MM-yyyy HH:mm", 
                       CultureInfo.InvariantCulture, 
                       DateTimeStyles.None, 
                       out selectedDateTime);
        
        if (success) 
            return selectedDateTime;
        else
            return null;
        
    }
    public void createAppointment()
    {
        DateTime? selectedDate = inputDate();
        Console.WriteLine(selectedDate);
    }
    public string selectOption(string commandGroup="")
    {
        if (commandGroup != "")
        {
            Console.Write("["+commandGroup+"] ");
        }
        Console.Write("Please enter a command: ");
        string? input = Console.ReadLine();
        if (input is null)
        {
            throw new NullInputException("Null value as input");
        }
        return input.ToLower().Trim();

    }

    public void manageAppointments()
    {
        Console.Clear();
        System.Console.WriteLine("");

        printCommands(this.ManageAppointmentsCommands);
        while (true){
            string selectedOption = selectOption("Manage appointments");
            if (selectedOption == "ca" || selectedOption == "create appointment")
            {
                createAppointment();
            }
            else if (selectedOption == "ua" || selectedOption == "update appointment")
            {
                Console.WriteLine("wip2");
            }
            else if (selectedOption == "va" || selectedOption == "view appointments")
            {
                Console.WriteLine("wip3");
            }
            else if (selectedOption == "da" || selectedOption == "delete appointment")
            {
                Console.WriteLine("wip4");
            }
            else if (selectedOption == "help")
            {
                printCommands(this.ManageAppointmentsCommands);
            }
            else if (selectedOption == "return")
            {
                Console.WriteLine("Returning...");
                Console.WriteLine("");
                break;
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

    public void printCommands(List<string> commands)
    {
        Console.WriteLine("Available commands: ");
        foreach (string command in commands)
        {
            Console.WriteLine(command);
        }
    }

    public override void Start()
    {
        Console.Clear();
        System.Console.WriteLine("");

        printCommands(this.MainCommands);
        while (true){
            string selectedOption = selectOption();
            if (selectedOption == "ma" || selectedOption == "manage appointments")
            {
                manageAppointments();
            }
            else if (selectedOption == "help")
            {
                printCommands(this.MainCommands);
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