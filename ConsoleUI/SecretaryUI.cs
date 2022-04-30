namespace Hospital;
using MongoDB.Driver;
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
    public List<string> CRUDCommands {get; private set;} = new List<string> {"Read list", "Create", "Read", "Update", "Delete", "Back"};

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
        UserRepository ur = _hospital.UserRepo;
        var usersGet = ur.GetUsers();
        var matchingUsers = from user in usersGet.AsQueryable() select user;

        foreach(var p in matchingUsers){
            users.Add(p);
        }

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
            System.Console.WriteLine(String.Format("| {0,-21} | {1,-20} | {2, -40} |", user.Person.FirstName, user.Person.LastName, user.Email));
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
        UserRepository ur = _hospital.UserRepo;
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
            Patient patient = new Patient(email, lastName);
            // FIXME: ADD PATIENT TO REPO!!!!!
            ur.AddUser(email, password, patient, Role.PATIENT);
        }
        printCommands(CRUDCommands);
    }

    public void ReadUserPatient(){
        Console.Clear();
        UserRepository ur = _hospital.UserRepo;
        System.Console.Write("Enter the user mail to view his data: ");
        string? email = Console.ReadLine();
        if (email is null)
        {
            throw new NullInputException("Null value as input");
        }
        var user = ur.GetUser(email);
        System.Console.WriteLine("Email : " + user.Email.ToString());
        System.Console.WriteLine("Password : " + user.Password.ToString());
        System.Console.WriteLine("First Name : " + user.Person.FirstName.ToString());
        System.Console.WriteLine("Last Name : " + user.Person.LastName.ToString());
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
        UserRepository ur = _hospital.UserRepo;
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
            ur.UpdateUserEmail(email,emailNew);
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
            ur.UpdateUserEmail(email,passwordNew);
        }
        Console.Clear();
        printCommands(CRUDCommands);
    }

    public void DeleteUserPatient()
    {
        Console.Clear();
        UserRepository ur = _hospital.UserRepo;
        System.Console.Write("Enter the user mail to delete: ");
        string? email = Console.ReadLine();
        if (email is null)
        {
            throw new NullInputException("Null value as input");
        }
        ur.DeleteUser(email);
        Console.Clear();
        printCommands(CRUDCommands);
    }
}

