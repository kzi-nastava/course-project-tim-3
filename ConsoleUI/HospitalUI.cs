namespace Hospital;
public class HospitalUI : ConsoleUI
{
    public HospitalUI(Hospital _hospital) : base(_hospital) {}

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

    public override void Start()
    {
        // _hospital.UserRepo.AddUser("email1", "password", "firstName", "firstName", Role.PATIENT); //TEST      
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
                var ui = new PatientUI(this._hospital, this._user);
                ui.Start();
                break;
            case Role.SECRETARY:
            var ui = new SecretaryUI(this._hospital, this._user);
                ui.Start();
                break;

            default:
                System.Console.WriteLine("SOMETHING WENT HORRIBLY WRONG. TERMINATING");
                break;
        }
    }

    public void AddUser(string email, string password, Person person, Role role)
    { // TODO: DELETE
        _hospital.UserRepo.AddUser(email, password, person, role);
    }
}