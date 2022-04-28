namespace Hospital;

public class SecretaryUI : ConsoleUI
{
    public SecretaryUI(Hospital _hospital, User? _user) : base(_hospital) 
    {
        this._user = _user;
    }

    public List<string> Commands {get; private set;} = new List<string> {"Create", "Read", "Update", "Delete"};

    public override void Start(){}

    public void printCommands()
    {
        Console.WriteLine("Available commands: ");
        foreach (string command in this.Commands)
        {
            Console.WriteLine(command);
        }
    }


}