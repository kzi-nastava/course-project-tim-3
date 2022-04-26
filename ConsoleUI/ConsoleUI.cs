namespace Hospital;
class ConsoleUI
{
    private Hospital _hospital = new Hospital();
    private User? _user;

    private bool TryLogin()
    {
        System.Console.Write("input email >> ");
        var email = Console.ReadLine();
        System.Console.Write("input password >> ");
        var password = Console.ReadLine();
        if (email is null || password is null) throw new Exception("AAAAAA"); // TODO: make better exception
        _user = _hospital.Login(email, password);
        if (_user is null)
        {
            System.Console.WriteLine("NO SUCH USER!! PLEASE TRY AGAIN"); 
        }
        return _user is not null;
    }

    public void Start()
    {
        var success = false;
        while (!success)
        {
            success = TryLogin();
        }
        Console.Clear();
        System.Console.WriteLine("Welcome, " + _user?.Person.FirstName + "!");
        // TODO: spawn UIs below
        switch (_user?.Role)
        {
            case Role.DIRECTOR:
            case Role.DOCTOR:
            case Role.PATIENT:
            case Role.SECRETARY:
                break;

            default:
                System.Console.WriteLine("SOMETHING WENT HORRIBLY WRONG. TERMINATING");
                break;
        }
    }

    public void AddUser(string email, string password, string firstName, string lastName, Role role)
    { // TODO: DELETE
        _hospital.UserRepo.AddUser(email, password, firstName, lastName, role);
    }
}