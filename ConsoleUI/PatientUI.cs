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

[System.Serializable]
public class GetOutException : System.Exception
{
    public GetOutException() { }
    public GetOutException(string message) : base(message) { }
    public GetOutException(string message, System.Exception inner) : base(message, inner) { }
    protected GetOutException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
public class PatientUI : ConsoleUI
{
    public List<string> MainCommands {get; private set;} = new List<string> {"ma / manage checkups","exit","help"};
    public List<string> ManageCheckupsCommands {get; private set;} = new List<string>
    {"cc / create checkup",
    "uc / update checkup",
    "va / view appointments",
    "dc / delete checkup",
    "return",
    "exit",
    "help"};

    //there might be a better way to set opening time, only time will be used
    //those times should be stored somewhere else
    DateTime openingTime = new DateTime(2000, 10, 20, 9, 0, 0);
    DateTime closingTime = new DateTime(2000, 10, 20, 17, 0, 0);
    DateTime now = DateTime.Now;
    TimeSpan checkupDuration = new TimeSpan(0,0,15,0);

    

    public PatientUI(Hospital _hospital, User? _user) : base(_hospital) 
    {
        this._user = _user;
    }

    public void RegisterCheckup(Checkup c)
    {

    }

    public int SelectIndex(string message){

        Console.Write(message);
        string? input = Console.ReadLine();
        if (input is null)
        {
            throw new NullInputException("Null value as input");
        }

        int selectedIndex;
        try
        {
            selectedIndex = Int32.Parse(input);
        }
        catch (FormatException)
        {
            throw;
        }

        return selectedIndex;
    }

    public Specialty SelectSpecialty()
    {
        Console.WriteLine("Specialities");
        foreach (Specialty spec in Specialty.GetValues(typeof(Specialty)))
            {
                Console.WriteLine(spec);
            }

        Console.Write("Please enter a speciality: ");
        string? input = Console.ReadLine();
        if (input is null)
        {
            throw new NullInputException("Null value as input");
        }

        input = input.Trim().ToUpper();

        switch (input)
        {
            case "DERMATOLOGY":
                return Specialty.DERMATOLOGY;
            case "RADIOLOGY":
                return Specialty.RADIOLOGY;
            case "STOMATOLOGY":
                return Specialty.STOMATOLOGY;
            case "OPHTHALMOLOGY":
                return Specialty.OPHTHALMOLOGY;
            case "FAMILY_MEDICINE":
                return Specialty.FAMILY_MEDICINE;
            default:
                throw new GetOutException("None of the specialities selected");
        }
    }
    public DateTime? selectDate ()
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

        if (DateTime.Compare(result.Date, now.Date) == -1 )
        {
            Console.WriteLine("Error - date is in past. Aborting...");    
            return null;
        }
        
        // time selection

        int checkupIndex = 0;
        DateTime iterationTime = openingTime;
        
        while (iterationTime.TimeOfDay != closingTime.TimeOfDay)
        {
            Console.WriteLine(checkupIndex + " - " + iterationTime.ToString("HH:mm"));
            iterationTime = iterationTime.Add(checkupDuration);
            checkupIndex += 1;
        }

        //while loop will add a sufficient "1" at the end of the loop
        checkupIndex -= 1;

        int selectedIndex = -1;
        try
        {
            selectedIndex = SelectIndex("Please enter a number from the list: ");
        }
        catch (Exception ex)            
        {                
            if (ex is NullInputException)
            {
                Console.WriteLine("Error - wrong input. Aborting...");
                return null;
            }
            else if (ex is FormatException)
            {
                Console.WriteLine("Error - wrong number. Aborting...");
                return null;
            }
        }

        result = result.AddHours(openingTime.Hour);
        if (selectedIndex >= 0 && selectedIndex <= checkupIndex)
        {
            result = result.Add(selectedIndex*checkupDuration);
        }
        else
        {
            Console.WriteLine("Error - wrong number. Aborting...");
        }
        //TODO: The listed times shouldnt be the ones that expired

        if (DateTime.Compare(result, now) == -1 )
        {
            Console.WriteLine("Selected time already expired. Aborting...");    
            return null;
        } 

        return result;
    }

    public void createCheckup()
    {
        DateTime? selectedDate = selectDate();
        if (selectedDate is null)
        {
            return;
        }

        Console.WriteLine("You have selected the following date - "+ selectedDate);

        Specialty selectedSpecialty;
        try
        {
            selectedSpecialty = SelectSpecialty();
        }
        catch (GetOutException)
        {
           Console.WriteLine("Error - selected speciality doesnt exist. Aborting...");
           return;
        }

        List<Doctor> suitableDoctors =  _hospital.DoctorRepo.GetDoctorBySpecialty(selectedSpecialty);

        if (suitableDoctors.Count == 0)
        {
            Console.WriteLine("No doctors found in selected specialty.");
            return;
        }

        for (int i=0; i<suitableDoctors.Count; i++)
        {
            Console.WriteLine(i+" - "+suitableDoctors[i].ToString());
        }

        int selectedIndex = -1;
        try
        {
            selectedIndex = SelectIndex("Please enter a number from the list: ");
        }
        catch (Exception ex)            
        {                
            if (ex is NullInputException)
            {
                Console.WriteLine("Error - wrong input. Aborting...");
                return;
            }
            else if (ex is FormatException)
            {
                Console.WriteLine("Error - wrong number. Aborting...");
                return;
            }
        }

        if (selectedIndex < 0 || selectedIndex >= suitableDoctors.Count)
        {
            Console.WriteLine("Error - wrong number. Aborting...");
            return;
        }

        if (suitableDoctors[selectedIndex].CheckIfFree((DateTime)selectedDate) is false)
        {
            Console.WriteLine("Checkup already taken.");
            return;
        }
        
        //TODO: Might want to create an additional expiry check for checkup timedate

        //public Checkup(DateTime timeAndDate, MongoDBRef patient, MongoDBRef doctor, string anamnesis)
        Checkup newCheckup = new Checkup(
            (DateTime)selectedDate,
            (Patient)this._user.Person,
            suitableDoctors[selectedIndex],
            checkupDuration,
            "no anamnesis");

        //TODO: this function is temporary
        RegisterCheckup(newCheckup);
        
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

    public void manageCheckups()
    {
        Console.Clear();
        System.Console.WriteLine("");

        printCommands(this.ManageCheckupsCommands);
        while (true){
            string selectedOption = selectOption("Manage checkups");
            if (selectedOption == "cc" || selectedOption == "create checkup")
            {
                createCheckup();
            }
            else if (selectedOption == "uc" || selectedOption == "update checkup")
            {
                Console.WriteLine("wip2");
            }
            else if (selectedOption == "va" || selectedOption == "view appointments")
            {
                Console.WriteLine("wip3");
            }
            else if (selectedOption == "dc" || selectedOption == "delete checkup")
            {
                Console.WriteLine("wip4");
            }
            else if (selectedOption == "help")
            {
                printCommands(this.ManageCheckupsCommands);
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
            if (selectedOption == "ma" || selectedOption == "manage checkups")
            {
                manageCheckups();
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