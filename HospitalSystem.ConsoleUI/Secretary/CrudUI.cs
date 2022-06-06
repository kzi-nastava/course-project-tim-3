using System.Text.RegularExpressions;
using HospitalSystem.Core;
using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem.ConsoleUI;
[System.Serializable]

public class CrudUI : ConsoleUI
{

    public CrudUI(Hospital hospital) : base(hospital)
    {
        // _loadedBatches = _hospital.EquipmentRepo.GetAll().ToList();
    }

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("INPUT OPTIONS:");
             System.Console.WriteLine("   1. Read patients-(rps)");
             System.Console.WriteLine("   2. Create patient-(cp)");
             System.Console.WriteLine("   3. Read patient-(rp)");
             System.Console.WriteLine("   4. Update patient-(up)");
             System.Console.WriteLine("   5. Delete patient-(dp)");
             System.Console.WriteLine("   6. Blocked patients-(bps)");
             System.Console.WriteLine("   7. Block patient-(bp)");
             System.Console.WriteLine("   8. Quit-(q)");
             System.Console.WriteLine("   9. Exit-(x)");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                if (choice == "read patients" || choice == "rps")
                {
                    System.Console.Write("BBB");
                    PagesUI pages = new PagesUI(_hospital);
                    pages.Start();
                }
                else if (choice == "create patient" || choice == "cp")
                {
                    CreateUserPatient();
                }
                else if (choice == "read patient" || choice == "rp")
                {
                    ReadUserPatient();
                }
                else if (choice == "update patient" || choice == "up")
                {
                    updateUserPatient();
                }
                else if (choice == "delete patient" || choice == "dp")
                {
                    DeleteUserPatient();
                }
                else if (choice == "blocked patients" || choice == "bps")
                {
                    SelectBlockedPatients();
                }
                else if (choice == "block patient" || choice == "bp")
                {
                    BlockUserPatients();
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
                    System.Console.WriteLine("INVALID INPUT - READ THE AVAILABLE COMMANDS!");
                    System.Console.Write("INPUT ANYTHING TO CONTINUE >> ");
                    ReadSanitizedLine();
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " INPUT ANYTHING TO CONTINUE >> ");
                ReadSanitizedLine();
            }
            catch (FormatException e)
            {
                System.Console.Write(e.Message + " INPUT ANYTHING TO CONTINUE >> ");
                ReadSanitizedLine();
            }
        }
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
            // ur.AddOrUpdateUser(new User(email, password,patient,Role.PATIENT));
            us.Upsert(new User(email, password,patient,Role.PATIENT));
        }
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
    }

    public void updateUserPatient()
    {
        Console.Clear();
        UserService us = _hospital.UserService;
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
            us.UpdateEmail(email,emailNew);
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
            us.UpdatePassword(email,passwordNew);
        }
        Console.Clear();
    }

    public void DeleteUserPatient()
    {
        Console.Clear();
        UserService us = _hospital.UserService;
        System.Console.Write("Enter the user mail to delete: ");
        string? email = Console.ReadLine();
        if (email is null)
        {
            throw new NullInputException("Null value as input");
        }
        us.Delete(email);
        Console.Clear();
    }

    public void BlockUserPatients()
    {
        Console.Clear();
        UserService us = _hospital.UserService;
        System.Console.Write("Enter the user mail to block: ");
        string? email = Console.ReadLine();
        if (email is null)
        {
            throw new NullInputException("Null value as input");
        }
        us.BlockPatient(email);
        Console.Clear();
    }
    public void SelectBlockedPatients()
    {
        Console.Clear();
        UserService us = _hospital.UserService;
        IQueryable<User> blockedUsers = us.GetAllBlocked();
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
         us.UnblockPatient(email);
        Console.Clear();
    }

    //MAKE BETTER CONSOLE INTERFACE
    public void CheckRequests(){
        Console.Clear();
        CheckupChangeRequestRepository cr = _hospital.CheckupChangeRequestRepo;
        var requestsGet = cr.GetAllAsQueryable();
        // List<User> requests = new List<User>();
        var matchingRequests = from request in requestsGet.AsQueryable() select request;

        int buffer = 1;
        foreach(var m in matchingRequests){
            Patient pat = _hospital.PatientRepo.GetPatientById((ObjectId) m.Checkup.Patient.Id);
            Doctor doc = _hospital.DoctorRepo.GetById((ObjectId) m.Checkup.Doctor.Id);
            System.Console.WriteLine("Index ID: " + buffer);
            System.Console.WriteLine("ID: " + m.Id.ToString());
            System.Console.WriteLine("Patient: " +  pat.FirstName + " " + pat.LastName);
            System.Console.WriteLine("Doctor: " +  doc.FirstName + " " + doc.LastName);
            System.Console.WriteLine("Start time: " + m.Checkup.DateRange.Starts);
            System.Console.WriteLine("End time: " + m.Checkup.DateRange.Ends);
            // System.Console.WriteLine("Duration: " + m.Checkup.DateRange);
            System.Console.WriteLine("RequestState: " + m.RequestState);
            System.Console.WriteLine("--------------------------------------------------------------------");
            System.Console.WriteLine();
            buffer = buffer + 1;
        }
        System.Console.Write("Enter id: ");
        string? stringId = Console.ReadLine();
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
    }
}