using HospitalSystem.Core;
using MongoDB.Bson;

namespace HospitalSystem.ConsoleUI;

public class CrudUI : HospitalClientUI
{
    public CrudUI(Hospital hospital) : base(hospital){}

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("INPUT OPTIONS:");
            System.Console.WriteLine("   1. View patients-(rps)");
            System.Console.WriteLine("   2. Create patient-(cp)");
            System.Console.WriteLine("   3. View patient-(rp)");
            System.Console.WriteLine("   4. Update patient-(up)");
            System.Console.WriteLine("   5. Delete patient-(dp)");
            System.Console.WriteLine("   6. Block patient-(bp)");
            System.Console.WriteLine("   7. Blocked patients-(bps)");
            System.Console.WriteLine("   8. Quit-(q)");
            System.Console.WriteLine("   9. Exit-(x)");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                if (choice == "read patients" || choice == "rps")
                {
                    PagesUI pages = new PagesUI(_hospital);
                    pages.Start();
                }
                else if (choice == "create patient" || choice == "cp")
                {
                    CreatePatientAccount();
                }
                else if (choice == "read patient" || choice == "rp")
                {
                    ViewPatientAccount();
                }
                else if (choice == "update patient" || choice == "up")
                {
                    UpdatePatientAccount();
                }
                else if (choice == "delete patient" || choice == "dp")
                {
                    DeletePatientAccount();
                }
                else if (choice == "block patient" || choice == "bp")
                {
                    BlockPatientAccount();
                }
                else if (choice == "blocked patients" || choice == "bps")
                {
                    BlockedPatientAccounts();
                }
                else if (choice == "quit" || choice == "q")
                {
                    throw new QuitToMainMenuException("From StartManageEquipments");
                }
                else if (choice == "exit" || choice == "x")
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    System.Console.WriteLine("Invalit input - read the available commands!");
                    System.Console.Write("Input anything to continue >> ");
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Input anything to continue >> ");
            }
            catch (FormatException e)
            {
                System.Console.Write(e.Message + " Input anything to continue >> ");
            }
            ReadSanitizedLine();
        }
    }

    public string EnterEmail(UserService us)
    {
        string email = ReadSanitizedLine();
        bool success = us.GetAll().Any(u => u.Email == email);
        if (!success)
        {
            throw new InvalidInputException("User with this email does not exist");
        }
        return email;
    }

    public void EnterNewEmail(UserService us)
    {
        System.Console.Write("Enter user email: ");
        string email = EnterEmail(us);
        System.Console.Write("Enter new email: ");
        string newEmail = ReadSanitizedLine();
        us.UpdateEmail(email,newEmail);
    }

    public void EnterNewPassword(UserService us)
    {
        System.Console.Write("Enter user email: ");
        string email = EnterEmail(us);
        System.Console.Write("Enter new password: ");
        string newPassword = ReadSanitizedLine(); 
        us.UpdatePassword(email,newPassword);
    }

    public void CreatePatientAccount()
    {   
        Console.Clear();
        UserService us = _hospital.UserService;

        System.Console.WriteLine("Enter the following data: ");

        System.Console.Write("email >> ");
        string email = ReadSanitizedLine();

        System.Console.Write("password >> ");
        string password = ReadSanitizedLine();

        System.Console.Write("first name >> ");
        string firstName = ReadSanitizedLine();

        System.Console.Write("last name >> ");
        string lastName = ReadSanitizedLine();

        Patient patient = new Patient(email, lastName, new MedicalRecord());
        _hospital.PatientService.Upsert(patient);
        us.Upsert(new User(email, password,patient,Role.PATIENT));

        System.Console.Write("Successfuly created a user. Type anything to get back to menu: ");
    }
    
    public void ViewPatientAccount()
    {
        Console.Clear();
        UserService us = _hospital.UserService;
        
        System.Console.Write("Enter the user mail to view his data: ");
        string email = EnterEmail(us);

        var user = us.Get(email);
        Patient pat = _hospital.PatientService.GetById((ObjectId) user.Person.Id);

        System.Console.WriteLine("Email : " + user.Email.ToString());
        System.Console.WriteLine("Password : " + user.Password.ToString());
        System.Console.WriteLine("First Name : " + pat.FirstName);
        System.Console.WriteLine("Last Name : " + pat.LastName);

        System.Console.Write("\nType anything to get to menu: ");
    }

    public void UpdatePatientAccount()
    {
        Console.Clear();
        UserService us = _hospital.UserService;

        System.Console.Write("Enter <email> or <password> depending of what you want update: ");
        string enter = ReadSanitizedLine();

        if(enter == "email")
        {
            EnterNewEmail(us);
        }
        else if(enter == "password")
        {
            EnterNewPassword(us);
        }
        else
        {
            throw new InvalidInputException("Invalid input");
        }
        System.Console.Write("\nnSuccessfuly updated. Type anything to get to menu: ");
    }

    public void DeletePatientAccount()
    {
        Console.Clear();
        UserService us = _hospital.UserService;

        System.Console.Write("Enter user email to delete: ");
        string email = EnterEmail(us);

        us.Delete(email);
        System.Console.Write("\nSuccessfuly deleted. Type anything to get to menu: ");
    }

    public void BlockPatientAccount()
    {
        Console.Clear();
        UserService us = _hospital.UserService;

        System.Console.Write("Enter the user mail to block: ");
        string email = EnterEmail(us);

        us.BlockPatient(email);
        System.Console.Write("\nSuccessfuly blocked. Type anything to get to menu: ");
    }

    public void BlockedPatientAccounts()
    {
        Console.Clear();
        UserService us = _hospital.UserService;
        List<User> blockedUsers = us.GetAllBlocked().ToList();

        System.Console.WriteLine("Blocked users: ");
        foreach(var blockedUser in blockedUsers)
        {
            Patient pat = _hospital.PatientService.GetById((ObjectId) blockedUser.Person.Id);
            System.Console.WriteLine(" << User: " + pat.FirstName.ToString() + " " + pat.LastName.ToString() + ", Email: " + blockedUser.Email.ToString() + " >> ");
        }

        System.Console.Write("\nEnter the user mail to unblock: ");
        string email = EnterEmail(us);

        us.UnblockPatient(email);
        System.Console.Write("\nSuccessfuly unblocked. Type anything to get to menu: ");
    }
}