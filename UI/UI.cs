namespace Hospital;
class HospitalUI
{
    private Hospital _hospital = new Hospital();
    private User? _user;

    private bool TryLogin()
    {
        System.Console.Write("input username >> ");
        var username = Console.ReadLine();
        System.Console.Write("input password >> ");
        var password = Console.ReadLine();
        if (username is null || password is null) throw new Exception("AAAAAA"); // TODO: make better exception
        _user = _hospital.Login(username, password);
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
        System.Console.WriteLine("Welcome, " + _user?.Username + "!");
    }

    public void AddUser(string username, string password, string firstName, string lastName, Role role)
    { // TODO: DELETE
        _hospital.UserRepo.AddUser(username, password, firstName, lastName, role);
    }
}