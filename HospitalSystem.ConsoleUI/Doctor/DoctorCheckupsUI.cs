using MongoDB.Bson;
using MongoDB.Driver;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

public class DoctorCheckupsUI : DoctorMainUI
{
    Doctor Doctor;
    public DoctorCheckupsUI(Hospital hospital, User user) : base(hospital, user) 
    { 
        Doctor = _hospital.DoctorService.GetById((ObjectId)_user.Person.Id);
    }
    public override void Start()
    {
        bool quit = false;
        while (!quit)
        {
            List<Checkup> checkups = ShowNextThreeDays();
            Console.Write("\nOptions:\n\n1. See patient info for checkup\n2. Start checkup\n3. Update checkup\n4. Delete checkup\n5. Back\n");
            Console.Write(">>");
            var input = ReadSanitizedLine().Trim();
            switch (input)
            {
                case "1":
                {
                    ShowInfoMenu(checkups);
                    break;
                }
                case "2":
                {
                    CheckupToStart(checkups);
                    break;
                }
                case "3":
                {
                    EditCheckupMenu(checkups);
                    break;
                }
                case "4":
                {
                    DeleteCheckupMenu(checkups);
                    break;
                }
                case "5":
                {
                    quit = true;
                    break;
                }
            }
        }
    }

    public void CheckupToStart(List<Checkup> checkups)
    {
        Console.Write("\nEnter checkup number >> ");
        var isNumber = int.TryParse(Console.ReadLine(), out int checkupNumber);
        if (isNumber == true && checkupNumber > 0 && checkupNumber <= checkups.Count())
        {
            new StartCheckupUI(_hospital, _user, checkups[checkupNumber-1]).Start();
        }
        else
        {
            Console.WriteLine("\nWrong input. Please enter one of the given checkups.");
        }
    }

    public List<Checkup> ShowNextThreeDays()
    {
        Console.WriteLine("\nThese are your checkups for the next 3 days:\n");
        List<Checkup> checkups = _hospital.AppointmentService.GetNotDoneCheckups(Doctor ,DateTime.Now);
        List<Operation> operations = _hospital.AppointmentService.GetNotDoneOperations(Doctor, DateTime.Now);
        checkups.AddRange(_hospital.AppointmentService.GetNotDoneCheckups(Doctor, DateTime.Today.AddDays(1)));
        operations.AddRange(_hospital.AppointmentService.GetNotDoneOperations(Doctor, DateTime.Today.AddDays(1)));
        checkups.AddRange(_hospital.AppointmentService.GetNotDoneCheckups(Doctor, DateTime.Today.AddDays(2)));
        operations.AddRange(_hospital.AppointmentService.GetNotDoneOperations(Doctor, DateTime.Today.AddDays(2)));
        PrintCheckups(checkups);
        PrintOperations(operations);
        return checkups;
    }

    public void EditCheckupMenu(List<Checkup> checkups)
    {
        Console.Write("\nEnter checkup number >> ");
        var isNumber = int.TryParse(Console.ReadLine(), out int checkupNumber);
        if (isNumber == true && checkupNumber >= 0 && checkupNumber <= checkups.Count())
        {
            Checkup checkup = checkups[checkupNumber-1];
            EditCheckup(checkup);
        }
        else
        {
            Console.WriteLine("\nWrong input. Please enter one of the given checkups.");
        }
    }

    public void EditCheckup(Checkup checkup)
    {
        Console.WriteLine("\n\nEdit checkup.\n");
        Console.Write("\nEdit options:\n\n1. Edit start date\n2. Edit Patient\n3. Back\n\n");
        Console.Write(">>");
        var editInput = Console.ReadLine();
        switch (editInput) 
        {
            case "1":
            {
                EditStartTime(checkup);
                break;
            }
            case "2":
            {
                EditCheckupPatient(checkup);
                break;
            }
        }
    }
    
    public void EditStartTime(Checkup checkup)
    {
        Console.Write("Enter new date >> ");
        string? date = Console.ReadLine();
        Console.Write("Enter new time >> ");
        string? time = Console.ReadLine();
        var newDateTime = DateTime.TryParse(date + " " + time, out DateTime newStartDate);
        if (newDateTime == true)
        {
            checkup.DateRange = new DateRange(newStartDate, newStartDate.Add(Checkup.DefaultDuration), allowPast: false);
            _hospital.ScheduleService.ScheduleCheckup(checkup);
            Console.WriteLine("\nEdit successfull");
        }
        else
        {
            Console.WriteLine("\nPlease enter valid date and time");
        }
    }

    public void EditCheckupPatient(Checkup checkup)
    {
        Console.Write("Enter new patient name>> ");
        string newName = ReadSanitizedLine();
        Console.Write("Enter new patient surname>> ");
        string newSurname = ReadSanitizedLine();
        Patient newPatient = _hospital.PatientService.GetPatientByFullName(newName,newSurname);
        if (newPatient != null)
        {
           checkup.Patient = new MongoDB.Driver.MongoDBRef("patients", newPatient.Id);
            _hospital.AppointmentService.UpsertCheckup(checkup);                
            Console.WriteLine("Edit successfull"); 
        }
        else
        {
            Console.WriteLine("\nNo such patient found.");
        }
    }

    public void ShowInfoMenu(List<Checkup> checkups)
    {
        Console.Write("\nEnter checkup number >> ");
        var isNumber = int.TryParse(Console.ReadLine(), out int checkupNumber);
        if (isNumber == true && checkupNumber >= 0 && checkupNumber <= checkups.Count())
        {
            ShowPatientInfo(checkups[checkupNumber-1]);
        }
        else
        {
            Console.WriteLine("\nWrong input. Please enter one of the given checkups.");
        }
    }

    public Patient ShowPatientInfo(Checkup checkup)
    {
        Patient patient = _hospital.PatientService.GetPatientById((ObjectId)checkup.Patient.Id);
        Console.Write("\n" + patient.ToString() + "\n");
        Console.Write(patient.MedicalRecord.ToString() + "\n");
        return patient;
    }

    public void DeleteCheckupMenu(List<Checkup> checkups)
    {
        Console.Write("\nEnter checkup number >> ");
        var isNumber = int.TryParse(Console.ReadLine(), out int checkupNumber);
        if (isNumber == true && checkupNumber >= 0 && checkupNumber <= checkups.Count())
        {
            _hospital.AppointmentService.DeleteCheckup(checkups[checkupNumber-1]);
            Console.WriteLine("Deletion successfull"); 
        }
        else
        {
            Console.WriteLine("\nWrong input. Please enter one of the given checkups.");
        }
    }
}