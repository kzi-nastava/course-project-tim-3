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

    //there might be a better way to set opening time, only time will be used
    //those times should be stored somewhere else
    DateTime openingTime = new DateTime(2000, 10, 20, 9, 0, 0);
    DateTime closingTime = new DateTime(2000, 10, 20, 17, 0, 0);

    

    public PatientUI(Hospital _hospital, User? _user) : base(_hospital) 
    {
        this._user = _user;
    }

    DateTime? selectDate ()
    {   
        DateTime result = new DateTime();

        // date selection

        Console.Write("Please enter a date in dd-MM-yyyy format: ");
        string? inputDate = Console.ReadLine();
        if (inputDate is null)
        {
            throw new NullInputException("Null value as input");
        }

        inputDate = inputDate.Trim();

        bool success = DateTime.TryParseExact(inputDate, 
                       "dd-MM-yyyy", 
                       CultureInfo.InvariantCulture, 
                       DateTimeStyles.None, 
                       out result);

        if (!success) 
        {
            Console.WriteLine("Error - wrong date. Aborting...");    
            return null;
        }
        
        // time selection

        int appointmentIndex = 0;
        TimeSpan appointmentDuration = new TimeSpan(0,0,15,0);
        DateTime iterationTime = openingTime;
        
        while (iterationTime.TimeOfDay != closingTime.TimeOfDay)
        {
            Console.WriteLine(appointmentIndex + " - " + iterationTime.ToString("HH:mm"));
            iterationTime = iterationTime.Add(appointmentDuration);
            appointmentIndex += 1;
        }

        //while loop will add a sufficient "1" at the end of the loop
        appointmentIndex -= 1;

        Console.Write("Please enter a number from the list: ");
        string? input = Console.ReadLine();
        if (input is null)
        {
            throw new NullInputException("Null value as input");
        }

        int selectedIndex;
        try
        {
            selectedIndex = Int32.Parse(input);
            Console.WriteLine(selectedIndex);
        }
        catch (FormatException)
        {
            Console.WriteLine("Error - wrong input. Aborting...");
            return null;
        }

        result = result.AddHours(9);
        if (selectedIndex >= 0 && selectedIndex <= appointmentIndex)
        {
            result = result.Add(selectedIndex*appointmentDuration);
        }
        else
        {
            Console.WriteLine("Error - wrong number. Aborting...");
        }

        return result;
    }

    public void createAppointment()
    {
        DateTime? selectedDate = selectDate();
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