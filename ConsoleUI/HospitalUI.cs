namespace HospitalSystem.ConsoleUI;

public class HospitalUI : ConsoleUI
{
    public HospitalUI(Hospital _hospital) : base(_hospital) {}

    private User? TryLogin()
    {
        System.Console.Write("Login:\n\n");
        System.Console.Write("input email >> ");
        var email = Console.ReadLine();
        System.Console.Write("input password >> ");
        var password = Console.ReadLine();
        if (email is null || password is null) throw new Exception("AAAAAA"); // TODO: make better exception
        var user = _hospital.Login(email, password);
        if (user is null)
        {
            System.Console.WriteLine("NO SUCH USER!! PLEASE TRY AGAIN"); 
        }
        return user;
    }

    public override void Start()
    {
        bool quit = false;
        while (!quit)
        {
            User? user = null;
            while (user is null)
            {
                user = TryLogin();
            }
            Console.Clear();
            System.Console.WriteLine("Welcome, " + user.Email + "!");
            switch (user.Role)
            {
                case Role.DIRECTOR:
                    DirectorUI dirUI = new DirectorUI(_hospital);
                    dirUI.Start();
                    break;
                case Role.DOCTOR:
                    DoctorUI doctorUI = new DoctorUI(_hospital, user);
                    doctorUI.Start();
                    break;
                case Role.PATIENT:
                {
                    var ui = new PatientUI(this._hospital, user);
                    ui.Start();
                    break;
                }
                case Role.SECRETARY:
                {
                    var secUI = new SecretaryUI(this._hospital, user);
                    secUI.Start();
                    break;
                }   
                default:
                    System.Console.WriteLine("SOMETHING WENT HORRIBLY WRONG. TERMINATING");
                    break;
            }
        }
    }
}