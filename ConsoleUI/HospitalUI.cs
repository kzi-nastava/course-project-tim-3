namespace Hospital;
public class HospitalUI : ConsoleUI
{
    public HospitalUI(Hospital _hospital) : base(_hospital) {}

    private bool TryLogin()
    {
        System.Console.Write("Login:\n\n");
        System.Console.Write("input email >> ");
        var email = Console.ReadLine();
        System.Console.Write("input password >> ");
        var password = Console.ReadLine();
        if (email is null || password is null) return false;//throw new Exception("AAAAAA"); // TODO: make better exception
        _user = _hospital.Login(email, password);
        if (_user is null)
        {
            System.Console.WriteLine("NO SUCH USER!! PLEASE TRY AGAIN"); 
        }
        return _user is not null;

        // REVIEW: let user quit while logging in
    }

    public override void Start()
    {
        // TODO: this doesn't belong, here. put it in service classes or something
        _hospital.RelocationRepo.ScheduleAll();
        _hospital.SimpleRenovationRepo.ScheduleAll();
        _hospital.SplitRenovationRepo.ScheduleAll();
        _hospital.MergeRenovationRepo.ScheduleAll();

        bool quit = false;
        while (!quit)
        {
        // _hospital.UserRepo.AddUser("email1", "password", "firstName", "firstName", Role.PATIENT); //TEST      
        var success = false;
        while (!success)
        {
            success = TryLogin();
        }
        Console.Clear();
        System.Console.WriteLine("Welcome, " + _user?.Email + "!");
        // TODO: spawn UIs below
        switch (_user?.Role)
        {
            case Role.DIRECTOR:
                DirectorUI dirUI = new DirectorUI(_hospital);
                dirUI.Start();
                break;
            case Role.DOCTOR:
                DoctorUI doctorUI = new DoctorUI(_hospital, _user);
                doctorUI.Start();
                break;
            case Role.PATIENT:
            {
                var ui = new PatientUI(this._hospital, this._user);
                ui.Start();
                break;
            }
            case Role.SECRETARY:
            {
                var secUI = new SecretaryUI(this._hospital, this._user);
                secUI.Start();
                break;
            }   
            default:
                System.Console.WriteLine("SOMETHING WENT HORRIBLY WRONG. TERMINATING");
                break;
        }
        }
    }

    public void AddUser(string email, string password, Person person, Role role)
    { // TODO: DELETE
        _hospital.UserRepo.AddOrUpdateUser(new User(email, password, person, role));
    }
}