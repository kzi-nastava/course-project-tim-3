using System.Globalization;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

public enum AppointmentInTime
    {
        PAST,
        FUTURE,
        ALL,
    }

[System.Serializable]
public class UserBlockedException : System.Exception
{
    public UserBlockedException() { }
    public UserBlockedException(string message) : base(message) { }
    public UserBlockedException(string message, System.Exception inner) : base(message, inner) { }
    protected UserBlockedException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class PatientUI : UserUI
{
    
    protected Patient _loggedInPatient;

    public PatientUI(Hospital hospital, User user) : base(hospital, user) 
    {
        _loggedInPatient = _hospital.PatientService.GetPatientById((ObjectId) user.Person.Id);
    }

    public override void Start()
    {
        if (_user.BlockStatus != Block.UNBLOCKED)
        {
            Console.WriteLine(@"
            Account blocked.
            Please contact secretary to unblock it.
            Press enter to continue ");
            ReadSanitizedLine();
            return;
        }

        while (true)
        {
            System.Console.WriteLine(@"
            Commands:
            ma - manage appointments
            vm - view medical record
            sd - search doctors
            mn - manage notifications
            ts - take surveys
            exit - quit the program

            ");
            string selectedOption = ReadSanitizedLine().Trim();
            
            try
            {
                if (selectedOption == "ma")
                {
                    ManageAppointmentsUI ui = new(_hospital,_user);
                    ui.Start();
                }
                else if (selectedOption == "vm")
                {
                   MedicalRecordUI ui = new(_hospital,_user);
                   ui.Start();
                }
                else if (selectedOption == "sd")
                {
                    DoctorSearchUI ui = new(_hospital,_user);
                    ui.Start();
                }
                else if (selectedOption == "mn")
                {
                    NotificationUI ui = new(_hospital,_user);
                    ui.Start();
                }
                else if (selectedOption == "ts")
                {
                    PatientSurveyUI ui = new(_hospital,_user);
                    ui.Start();
                }
                else if (selectedOption == "exit")
                {
                    Console.WriteLine("Exiting...");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Unrecognized command, please try again");
                }
            }
            catch(UserBlockedException e)
            {
                System.Console.WriteLine("Account blocked. Reason: "+ e.Message);
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public Checkup SelectCheckup ()
    {
        List<Checkup> checkups = _hospital.AppointmentService.GetFutureCheckupsByPatient(_loggedInPatient.Id);
        if (checkups.Count == 0)
        {
            throw new QuitToMainMenuException("No checkups.");
        }

        int selectedIndex = -1;
        try
        {
            System.Console.Write("Please enter a number from the list: ");
            selectedIndex = ReadInt(0, checkups.Count-1);
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            throw new QuitToMainMenuException("Wrong input");
        }

        return checkups[selectedIndex];
    }

    public string ConvertAppointmentToString(Appointment a)
    {
        string output = "";

        output += a.DateRange.Starts +" ";
        Doctor doctor = _hospital.DoctorService.GetById((ObjectId)a.Doctor.Id);
        output += doctor.ToString();

        return output;
    }

    public void ShowCheckups(AppointmentInTime checkupTime)
    {
        //unnecessary but code wouldnt compile
        List<Checkup> checkups = new List<Checkup>();
        switch (checkupTime)
        {
            case AppointmentInTime.ALL:
                checkups = _hospital.AppointmentService.GetCheckupsByPatient(_loggedInPatient.Id);
                break;   
            case AppointmentInTime.FUTURE:
                checkups = _hospital.AppointmentService.GetFutureCheckupsByPatient(_loggedInPatient.Id);
                break;
            case AppointmentInTime.PAST:
                checkups = _hospital.AppointmentService.GetPastCheckupsByPatient(_loggedInPatient.Id);
                break;
        }
        
        if (checkups.Count == 0)
        {
            Console.WriteLine("No checkups.");
            return;
        }
        for (int i = 0; i< checkups.Count; i++)
        {
            Console.WriteLine(i+" - "+ConvertAppointmentToString(checkups[i]));
        }
    }

    public void ShowOperations(AppointmentInTime operationTime)
    {
        //unnecessary but code wouldnt compile
        List<Operation> operations = new List<Operation>();
        switch (operationTime)
        {
            case AppointmentInTime.ALL:
                operations = _hospital.AppointmentService.GetOperationsByPatient(_loggedInPatient.Id);
                break;   
            case AppointmentInTime.FUTURE:
                operations = _hospital.AppointmentService.GetFutureOperationsByPatient(_loggedInPatient.Id);
                break;
            case AppointmentInTime.PAST:
                operations = _hospital.AppointmentService.GetPastOperationsByPatient(_loggedInPatient.Id);
                break;
        }
        
        if (operations.Count == 0)
        {
            Console.WriteLine("No operations.");
            return;
        }
        for (int i = 0; i< operations.Count; i++)
        {
            Console.WriteLine(i+" - "+ConvertAppointmentToString(operations[i]));
        }
    }
    public void ShowAppointments()
    {   
        Console.WriteLine("### Checkups ###");
        ShowCheckups(AppointmentInTime.FUTURE);
        Console.WriteLine("### Operations ###");
        ShowOperations(AppointmentInTime.FUTURE);

    }

    public Specialty SelectSpecialty()
    {
        Console.WriteLine("Specialities");
        foreach (Specialty spec in Specialty.GetValues(typeof(Specialty)))
        {
            Console.WriteLine(spec);
        }

        Console.Write("Please enter a speciality: ");
        string input = ReadSanitizedLine().Trim().ToUpper();

        switch (input)
        {
            case nameof(Specialty.DERMATOLOGY):
                return Specialty.DERMATOLOGY;
            case nameof(Specialty.RADIOLOGY):
                return Specialty.RADIOLOGY;
            case nameof(Specialty.STOMATOLOGY):
                return Specialty.STOMATOLOGY;
            case nameof(Specialty.OPHTHALMOLOGY):
                return Specialty.OPHTHALMOLOGY;
            case nameof(Specialty.FAMILY_MEDICINE):
                return Specialty.FAMILY_MEDICINE;
            default:
                throw new InvalidInputException("Speciality not recognized.");
        }
    }

    //takes a datetime with date part already set, and sets its time part
    public DateTime SelectTime(DateTime inputDate)
    {
        int highestCheckupIndex = 0;
        DateTime iterationTime = Globals.OpeningTime;
        
        while (iterationTime.TimeOfDay != Globals.ClosingTime.TimeOfDay)
        {
            Console.WriteLine(highestCheckupIndex + " - " + iterationTime.ToString("HH:mm"));
            iterationTime = iterationTime.Add(Globals._checkupDuration);
            highestCheckupIndex += 1;
        }

        System.Console.Write("Please enter a number from the list: ");
        int selectedIndex = ReadInt(0, highestCheckupIndex-1);

        inputDate = inputDate.AddHours(Globals.OpeningTime.Hour);
        inputDate = inputDate.Add(selectedIndex*Globals._checkupDuration);

        return inputDate;
    }

    public DateTime SelectDate()
    {
        string inputDate = ReadSanitizedLine().Trim();

        bool success = DateTime.TryParseExact(inputDate, 
                       "dd-MM-yyyy", 
                       CultureInfo.InvariantCulture, 
                       DateTimeStyles.None, 
                       out DateTime result);

        if (!success) 
        {
            throw new InvalidInputException("Wrong date entered.");  
        }

        if (DateTime.Compare(result.Date, DateTime.Now.Date) == -1 )
        {
            throw new InvalidInputException("The date entered is in past.");
        }
        return result;
    }

    public DateTime SelectDateAndTime ()
    {   
        Console.Write("Please enter a date in dd-MM-yyyy format: ");
        DateTime result = SelectDate();

        result = SelectTime(result);
       
        //TODO: The listed times shouldnt be the ones that expired

        if (DateTime.Compare(result, DateTime.Now) == -1 )
        {
             throw new InvalidInputException("Selected date and time expired.");
        } 

        return result;
    }

    public Doctor? SelectDoctor(Specialty selectedSpecialty)
    {
        List<Doctor> suitableDoctors =  _hospital.DoctorService.GetManyBySpecialty(selectedSpecialty);
        if (suitableDoctors.Count == 0)
        {
            Console.WriteLine("No doctors found in selected specialty.");
            ReadSanitizedLine();
            return null;
        }

        for (int i=0; i<suitableDoctors.Count; i++)
        {
            Console.WriteLine(i+" - "+suitableDoctors[i].ToString());
        }

        int selectedIndex = -1;
        try
        {
            System.Console.Write("Please enter a number from the list: ");
            selectedIndex = ReadInt(0, suitableDoctors.Count-1);
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            return null;
        }

        return suitableDoctors[selectedIndex];
    }

    public void CreateCheckup(Doctor ?overrideDoctor = null)
    {

        //TODO: change this
        bool nextWillBlock = _hospital.PatientService.WillNextCRUDOperationBlock(CRUDOperation.CREATE, _loggedInPatient);
        if (nextWillBlock)
        {
            Console.WriteLine("Warning! Any additional checkup creation will result in account block!");
        }

        DateTime selectedDate;
        try
        {
            selectedDate = SelectDateAndTime();
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            return;
        }
        
        Console.WriteLine("You have selected the following date - "+ selectedDate);
        Doctor? selectedDoctor;
        if (overrideDoctor is null)
        {
            Specialty selectedSpecialty;
            try
            {
                selectedSpecialty = SelectSpecialty();
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Aborting...");
                return;
            }

            selectedDoctor = SelectDoctor(selectedSpecialty);
            if (selectedDoctor == null)
            {
                return;
            }
        }
        else
        {
            selectedDoctor = overrideDoctor;
        }

        //TODO: Might want to create an additional expiry check for checkup timedate
        Checkup newCheckup = new Checkup(
            selectedDate,
            new MongoDB.Driver.MongoDBRef("patients", _user.Person.Id),
            new MongoDB.Driver.MongoDBRef("doctors", selectedDoctor.Id),
            "no anamnesis");
        
        if (!_hospital.AppointmentService.IsDoctorAvailable(newCheckup.DateRange, selectedDoctor))
        {
            Console.WriteLine("Checkup already taken.");
            return;
        }
        Console.WriteLine("Checkup is free to schedule");
        
        _hospital.AppointmentService.UpsertCheckup(newCheckup);
        Console.WriteLine("Checkup created");
        
        _hospital.PatientService.LogChange(CRUDOperation.CREATE,_loggedInPatient);
        if (nextWillBlock)
        {
            _hospital.UserService.BlockUser(_user);
            throw new UserBlockedException("Creating too many checkups.");
        }
    }
}