using HospitalSystem.Core;
using HospitalSystem.ConsoleUI.Director;

namespace HospitalSystem.ConsoleUI;

public class HospitalUI : HospitalClientUI
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
        var user = _hospital.UserService.Login(email, password);
        if (user is null)
        {
            System.Console.WriteLine("No such user. Please try again"); 
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
            HospitalClientUI myUI;
            switch (user.Role)
            {
                case Role.DIRECTOR:
                    myUI = new DirectorUI(_hospital);
                    break;
                case Role.DOCTOR:
                    myUI = new DoctorMainUI(_hospital, user);
                    break;
                case Role.PATIENT:
                    myUI = new PatientUI(_hospital, user);
                    break;
                case Role.SECRETARY:
                    myUI = new SecretaryUI(_hospital);
                    break;
                default:
                    System.Console.WriteLine("Something went horribly wrong. Terminating");
                    return;
            }
            myUI.Start();
        }
    }
}