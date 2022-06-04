using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

[System.Serializable]
public class NotMatchingtException : System.Exception
{
    public NotMatchingtException() { }
    public NotMatchingtException(string message) : base(message) { }
    public NotMatchingtException(string message, System.Exception inner) : base(message, inner) { }
    protected NotMatchingtException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class NullInputException : System.Exception
{
    public NullInputException() { }
    public NullInputException(string message) : base(message) { }
    public NullInputException(string message, System.Exception inner) : base(message, inner) { }
    protected NullInputException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class SecretaryUI : UserUI
{   
    public SecretaryUI(Hospital hospital, User user) : base(hospital, user) { }

    public List<string> Commands {get; private set;} = new List<string> {"Options", "Help", "Exit"};
    public List<string> CRUDCommands {get; private set;} = new List<string> {"Read list", "Create", "Read", "Update", "Delete", 
    "Select blocked", "Block Patient", "Check Requests", "Back"};

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
        Console.WriteLine("");
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
        while (true){
            string selectedOption = selectOption();
            if (selectedOption == "readlist"){
                readListUserPatients();
            }
            else if (selectedOption == "create"){
                CreateUserPatient();
            }
            else if (selectedOption == "read"){
                ReadUserPatient();
            }
            else if (selectedOption == "update"){
                updateUserPatient();
            }
            else if (selectedOption == "delete"){
                DeleteUserPatient();
            }
            else if (selectedOption == "selectblocked"){
                SelectBlockedPatients();
            }
            else if (selectedOption == "blockpatient"){
                BlockUserPatients();
            }
            else if (selectedOption == "cr"){
                CheckRequests();
            }
            else if (selectedOption == "back")
            {
                Console.Clear();
                printCommands(Commands);
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Unrecognized command, please try again");
                printCommands(CRUDCommands);
            }
        }
    }

    public void readListUserPatients()
    {   
        Console.Clear();
        List<User> users = new List<User>();
        UserService us = _hospital.UserService;
        var matchingUsers = us.GetAll();

        foreach(var p in matchingUsers){
            if (p.Role == Role.PATIENT){
                users.Add(p);
            }
        }

        Console.WriteLine(users.Count().ToString()); 

        int usersListSize = users.Count();
        int startIndex = 0;
        int endIndex = 10;

        header();
        userPages(users, startIndex, endIndex);   

        while(true){
            string selectedOption = selectOption();
            Console.Clear();
            if (selectedOption == "left")
            {
                startIndex = startIndex-10;
                endIndex = endIndex-10;
                if(startIndex >= 0)
                { 
                    header();
                    userPages(users, startIndex, endIndex);
                }
                else{
                    startIndex = startIndex+10;
                    endIndex = endIndex+10;
                    header();
                    userPages(users, startIndex, endIndex);
                    System.Console.WriteLine("There are no more previous pages");
                }
            }
            else if(selectedOption == "right")
            {   
                startIndex = startIndex+10;
                endIndex = endIndex+10;
                if(endIndex <= usersListSize)
                {   
                    header();
                    userPages(users, startIndex, endIndex);
                }
                else if((10 - (endIndex-usersListSize)) >= 0){
                    int newEndIndex = 10 - (endIndex-usersListSize);
                    System.Console.WriteLine(newEndIndex.ToString());
                    header();
                    userPages(users, startIndex, usersListSize);
                }
                else{
                    header();
                    userPages(users, startIndex-10, usersListSize);
                    startIndex = startIndex-10;
                    endIndex = endIndex-10;
                    System.Console.WriteLine("There are no more next pages");
                }
            }
            else if(selectedOption == "back"){
                printCommands(CRUDCommands);
                break;
            }
        }
    }

    public void header()
    {
        System.Console.WriteLine("__________________________________________________________________________________________");
        System.Console.WriteLine("|                       |                      |                                          |");
        System.Console.WriteLine("|       First Name      |      Last Name       |                   Email                  |");
        System.Console.WriteLine("|_______________________|______________________|__________________________________________|");
        System.Console.WriteLine("|                       |                      |                                          |");

    }

    public void userPages(List<User> usersList, int startIndex, int endIndex)
    {   
        int i;
        for(i = startIndex; i < endIndex; i++ ){
            var user = usersList.ElementAt(i);
            Patient pat = _hospital.PatientRepo.GetPatientById((ObjectId) user.Person.Id);
            System.Console.WriteLine(String.Format("| {0,-21} | {1,-20} | {2, -40} |", pat.FirstName, pat.LastName, user.Email));
        }

        System.Console.WriteLine("|_______________________|______________________|__________________________________________|");
        System.Console.WriteLine("Choose:");
        System.Console.WriteLine("       <Left> (previous 10 users)");
        System.Console.WriteLine("       <Right> (next 10 users)");
        System.Console.WriteLine("       <Back>");
        System.Console.WriteLine("");

    }

    public void CreateUserPatient()
    {   
        Console.Clear();
        UserService us = _hospital.UserService;
        System.Console.WriteLine("Enter the following data: ");
        System.Console.Write("email >> ");
        string? email = Console.ReadLine();
        if (email is null)
        {
            throw new NullInputException("Null value as input");
        }
        System.Console.Write("password >> ");
        string? password = Console.ReadLine();
        if (password is null)
        {
            throw new NullInputException("Null value as input");
        }
        System.Console.Write("first name >> ");
        string? firstName = Console.ReadLine();
        if (firstName is null)
        {
            throw new NullInputException("Null value as input");
        }
        System.Console.Write("last name >> ");
        string? lastName = Console.ReadLine();
        if (lastName is null)
        {
            throw new NullInputException("Null value as input");
        }
        System.Console.WriteLine(email);
        if(email == "back"){
            Console.Clear();
            System.Console.WriteLine("Returning...");
        }
        else if(password == "back"){
            Console.Clear();
            System.Console.WriteLine("Returning...");

        }
        else if(firstName == "back"){
            Console.Clear();
            System.Console.WriteLine("Returning...");

        }
        else if(lastName == "back"){
            Console.Clear();
            System.Console.WriteLine("Returning...");
        }
        else{
            Console.Clear();
            Patient patient = new Patient(email, lastName, new MedicalRecord());
            _hospital.PatientRepo.AddOrUpdatePatient(patient);
            us.Upsert(new User(email, password,patient,Role.PATIENT));
        }
        printCommands(CRUDCommands);
    }

    public void ReadUserPatient(){
        Console.Clear();
        UserService us = _hospital.UserService;
        System.Console.Write("Enter the user mail to view his data: ");
        string? email = Console.ReadLine();
        if (email is null)
        {
            throw new NullInputException("Null value as input");
        }
        var user = us.Get(email);
        Patient pat = _hospital.PatientRepo.GetPatientById((ObjectId) user.Person.Id);
        System.Console.WriteLine("Email : " + user.Email.ToString());
        System.Console.WriteLine("Password : " + user.Password.ToString());
        System.Console.WriteLine("First Name : " + pat.FirstName);
        System.Console.WriteLine("Last Name : " + pat.LastName);
        System.Console.WriteLine("");
        System.Console.Write("Type back to get to menu: ");
        string? back = Console.ReadLine();

        while(back != "back"){
        Console.Clear();
        System.Console.Write("Type back to get to menu: ");
        back = Console.ReadLine();
        }

        Console.Clear();
        printCommands(CRUDCommands);
    }

    public void updateUserPatient()
    {
        Console.Clear();
        UserService ur = _hospital.UserService;
        System.Console.Write("Enter <email> or <password> depending of what you want update: ");
        
        string? enter = Console.ReadLine();
        if(enter == "email"){
            System.Console.Write("Enter users email: ");
            string? email = Console.ReadLine();
            if (email is null)
            {
                throw new NullInputException("Null value as input");
            }
            System.Console.Write("Enter new email: ");
            string? emailNew = Console.ReadLine();
            if (emailNew is null)
            {
                throw new NullInputException("Null value as input");
            }
            ur.UpdateEmail(email,emailNew);
        }
        else if(enter == "password"){
            System.Console.Write("Enter users password : ");
            string? email = Console.ReadLine();
            if (email is null)
            {
                throw new NullInputException("Null value as input");
            }
            System.Console.Write("Enter new password: ");
            string? passwordNew = Console.ReadLine();
            if (passwordNew is null)
            {
                throw new NullInputException("Null value as input");
            }
            ur.UpdateEmail(email,passwordNew);
        }
        Console.Clear();
        printCommands(CRUDCommands);
    }

    public void DeleteUserPatient()
    {
        Console.Clear();
        UserService ur = _hospital.UserService;
        System.Console.Write("Enter the user mail to delete: ");
        string? email = Console.ReadLine();
        if (email is null)
        {
            throw new NullInputException("Null value as input");
        }
        ur.Delete(email);
        Console.Clear();
        printCommands(CRUDCommands);
    }

    public void BlockUserPatients()
    {
        Console.Clear();
        UserService ur = _hospital.UserService;
        System.Console.Write("Enter the user mail to block: ");
        string? email = Console.ReadLine();
        if (email is null)
        {
            throw new NullInputException("Null value as input");
        }
        ur.BlockPatient(email);
        Console.Clear();
        printCommands(CRUDCommands);
    }
    public void SelectBlockedPatients()
    {
        Console.Clear();
        UserService ur = _hospital.UserService;
        var blockedUsers = ur.GetAllBlocked();
        System.Console.WriteLine("Blocked users(email): ");
        foreach(var b in blockedUsers){
            Patient pat = _hospital.PatientRepo.GetPatientById((ObjectId) b.Person.Id);
            System.Console.WriteLine(" << User: " + pat.FirstName.ToString() + " " + pat.LastName.ToString() + ", Email: " + b.Email.ToString() + " >> ");
        }
        System.Console.WriteLine();
        System.Console.Write("Enter the user mail to unblock: ");
        string? email = Console.ReadLine();
        if (email is null)
        {
            throw new NullInputException("Null value as input");
        }
         ur.UnblockPatient(email);
        Console.Clear();
        printCommands(CRUDCommands);
    }

    //MAKE BETTER CONSOLE INTERFACE
    public void CheckRequests(){
        Console.Clear();
        CheckupChangeRequestRepository cr = _hospital.CheckupChangeRequestRepo;
        var requestsGet = cr.GetAll();
        List<User> requests = new List<User>();
        var matchingRequests = from request in requestsGet.AsQueryable() select request;

        int buffer = 1;
        foreach(var m in matchingRequests){
            Patient pat = _hospital.PatientRepo.GetPatientById((ObjectId) m.Checkup.Patient.Id);
            Doctor doc = _hospital.DoctorService.GetById((ObjectId) m.Checkup.Doctor.Id);
            System.Console.WriteLine("Index ID: " + buffer);
            System.Console.WriteLine("ID: " + m.Id.ToString());
            System.Console.WriteLine("Patient: " +  pat.FirstName + " " + pat.LastName);
            System.Console.WriteLine("Doctor: " +  doc.FirstName + " " + doc.LastName);
            System.Console.WriteLine("Start time: " + m.Checkup.DateRange.Starts);
            System.Console.WriteLine("End time: " + m.Checkup.DateRange.Ends);
            System.Console.WriteLine("Duration: " + m.Checkup.DateRange.GetDuration());
            System.Console.WriteLine("RequestState: " + m.RequestState);
            System.Console.WriteLine("--------------------------------------------------------------------");
            System.Console.WriteLine();
            buffer = buffer + 1;
        }
        System.Console.Write("Enter id: ");
        string stringId = ReadSanitizedLine();
        int indexId = Int16.Parse(stringId);
        System.Console.Write("Enter state(approved, denied): ");
        string? stringState = Console.ReadLine();
        if (stringState is null)
        {
            throw new NullInputException("Null value as input");
        }
        if (stringState == "approved")
        {
            cr.UpdateRequest(indexId, RequestState.APPROVED);
        }
        if (stringState == "denied"){
        cr.UpdateRequest(indexId, RequestState.DENIED);
        }
        Console.Clear();
        printCommands(CRUDCommands);
    }
}

