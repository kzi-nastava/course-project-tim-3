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
    //there might be a better way to set opening time, only time will be used
    //those times should be stored somewhere else
    private DateTime _now = DateTime.Now;
    private TimeSpan _checkupDuration = new TimeSpan(0,0,15,0);
    private Patient _loggedInPatient;

    public PatientUI(Hospital hospital, User user) : base(hospital, user) 
    {
        _loggedInPatient = _hospital.PatientRepo.GetPatientById((ObjectId) user.Person.Id);
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
            exit - quit the program

            ");
            string selectedOption = ReadSanitizedLine().Trim();
            
            try
            {
                if (selectedOption == "ma")
                {
                    ManageAppointments();
                }
                else if (selectedOption == "vm")
                {
                    StartMedicalRecord();
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
            catch (InvalidInputException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public void StartAnamnesisSearch()
    {
        Console.Write("Please enter a search keyword: ");
        string keyword = ReadSanitizedLine().Trim();

        List<Checkup> filteredCheckups = _hospital.AppointmentService.SearchPastCheckups(_loggedInPatient.Id,keyword);

        if (filteredCheckups.Count == 0)
        {
            Console.WriteLine("No anamnesis found");
            return;
        }

        System.Console.WriteLine(@"
            Sort options:
            d - sort by date
            n - sort by doctors name
            s - sort by specialty
            ");
        
        //there is probably a better way to do n and s, but idk
        string sortSelection = ReadSanitizedLine().Trim();
        if (sortSelection == "d")
        {
            filteredCheckups.Sort((checkup1, checkup2)=> 
                DateTime.Compare(checkup1.DateRange.Starts, checkup2.DateRange.Ends));
        }
        else if (sortSelection == "n")
        {
            filteredCheckups.Sort(_hospital.AppointmentService.CompareCheckupsByDoctorsName);
        }
        else if (sortSelection == "s")
        {
            filteredCheckups.Sort(_hospital.AppointmentService.CompareCheckupsByDoctorsSpecialty);
        }

        foreach (Checkup checkup in filteredCheckups)
        {
           ShowCheckupsAnamnesis(checkup);
        }
    }

    public void StartMedicalRecord()
    {
        while (true)
        {
            System.Console.WriteLine(@"
            Commands:
            sc - show past checkups
            as - anamnesis search
            return - go to the previous menu
            exit - quit the program
            ");

            string selectedOption = ReadSanitizedLine().Trim();

            try
            {
                if (selectedOption == "sc")
                {
                    StartPastCheckups();
                }
                else if (selectedOption == "as")
                {
                    StartAnamnesisSearch();
                }
                else if (selectedOption == "return")
                {
                    Console.WriteLine("Returning...\n");
                    break;
                }
                else if (selectedOption == "exit")
                {
                    Console.WriteLine("Exiting...\n");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Unrecognized command, please try again");
                }
            }
            //this might create problems
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message);
            }
        }
    }

    public void StartAppointmentRUD()
    {
        while (true)
        {
            //Console.Clear();
            System.Console.WriteLine(@"
            Commands:
            sa - show appointments
            uc - update checkup
            dc - delete checkup
            return - go to the previous menu
            exit - quit the program

            ");

            string selectedOption = ReadSanitizedLine().Trim();
            try
            {
            
                if (selectedOption == "sa")
                {
                    ShowAppointments();
                }
                else if (selectedOption == "uc")
                {
                    UpdateCheckup();
                }
                else if (selectedOption == "dc")
                {
                    DeleteCheckup();
                }
                else if (selectedOption == "return")
                {
                    Console.WriteLine("Returning...\n");
                    break;
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
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public void ManageAppointments()
    {
        while (true){
            System.Console.WriteLine(@"
            Commands:
            cc - create checkup
            ccr - create checkup (with recommendations)
            va - view and manage appointments
            return - go to the previous menu
            exit - quit the program

            ");

            string selectedOption = ReadSanitizedLine().Trim();

            try
            {
                if (selectedOption == "cc")
                {
                    CreateCheckup();
                }
                else if (selectedOption == "ccr")
                {
                    CreateCheckupAdvanced();
                }
                else if (selectedOption == "va")
                {
                    StartAppointmentRUD();
                }
                else if (selectedOption == "return")
                {
                    Console.WriteLine("Returning...\n");
                    break;
                }
                else if (selectedOption == "exit")
                {
                    Console.WriteLine("Exiting...\n");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Unrecognized command, please try again");
                }
            }
            catch(UserBlockedException e)
            {
                throw;
            }
            //this might create problems, used to be generic exception
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message);
            }
        }
    }

    public void ShowCheckupsAnamnesis(Checkup checkup)
    {
        Doctor doctor = _hospital.DoctorService.GetById( (ObjectId)checkup.Doctor.Id );
        Console.WriteLine("[ " + checkup.DateRange.Starts + " " + doctor + " ] ");
        Console.WriteLine(checkup.Anamnesis);
        Console.WriteLine();
    }

    public void StartPastCheckups()
    {
        ShowCheckups(AppointmentInTime.PAST);
        List<Checkup> pastCheckups = _hospital.AppointmentService.GetPastCheckupsByPatient(_loggedInPatient.Id);
        int selectedIndex;
        try
        {
            System.Console.Write("To view checkup anamnesis please enter a number from the list: ");
            selectedIndex = ReadInt(0, pastCheckups.Count-1, "Number out of bounds!", "Number not recognized!");
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            throw new QuitToMainMenuException("Wrong input");
        }

        Checkup selectedCheckup = pastCheckups[selectedIndex];
        Console.WriteLine("Anamnesis: "+ selectedCheckup.Anamnesis);

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
            selectedIndex = ReadInt(0, checkups.Count-1, "Number out of bounds!", "Number not recognized!");
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            throw new QuitToMainMenuException("Wrong input");
        }

        return checkups[selectedIndex];
    }

    public void DeleteCheckup ()
    {
        bool nextWillBlock = _hospital.PatientService.WillNextCRUDOperationBlock(CRUDOperation.DELETE, _loggedInPatient);
        if (nextWillBlock)
        {
            Console.WriteLine("Warning! Any additional checkup deletion will result in account block!");
        }
        Checkup selectedCheckup;
        try
        {   
            ShowCheckups(AppointmentInTime.FUTURE);
            selectedCheckup = SelectCheckup();
        }
        catch (QuitToMainMenuException)
        {
            return;
        }

        if (selectedCheckup.DateRange.Starts < _now.AddDays(2))
        {
            CheckupChangeRequest newRequest = new CheckupChangeRequest(
                selectedCheckup,
                CRUDOperation.DELETE);
                Console.WriteLine("Checkup date is in less than 2 days from now. Change request sent.");
                _hospital.CheckupChangeRequestRepo.AddOrUpdate(newRequest);
        }
        else
        {
            _hospital.AppointmentService.DeleteCheckup(selectedCheckup);
            Console.WriteLine("Checkup deleted.");
        }

        _hospital.PatientService.LogChange(CRUDOperation.DELETE,_loggedInPatient);
        if (nextWillBlock)
        {
            _hospital.UserService.BlockUser(_user);
            throw new UserBlockedException("Deleting too many checkups.");
        }

    }

    public void UpdateCheckup()
    {

        bool nextWillBlock = _hospital.PatientService.WillNextCRUDOperationBlock(CRUDOperation.UPDATE, _loggedInPatient);
        if (nextWillBlock)
        {
            Console.WriteLine("Warning! Any additional checkup updating will result in account block!");
        }
        Checkup selectedCheckup;
        try
        {
            ShowCheckups(AppointmentInTime.FUTURE);
            selectedCheckup = SelectCheckup();
        }
        catch (QuitToMainMenuException)
        {
            return;
        }
        Console.WriteLine ("You have selected " + ConvertAppointmentToString(selectedCheckup));

        Doctor currentDoctor = _hospital.DoctorService.GetById((ObjectId)selectedCheckup.Doctor.Id);
        DateTime existingDate = selectedCheckup.DateRange.Starts;
        
        List<Doctor> alternativeDoctors =  _hospital.DoctorService.GetManyBySpecialty(currentDoctor.Specialty);
        alternativeDoctors.Remove(currentDoctor);
        Doctor newDoctor = currentDoctor;
        DateTime newDate = existingDate;

        // change doctor?

        Console.WriteLine("Change doctor? Enter yes or no: ");

        string changeDoctorOpinion = ReadSanitizedLine().Trim();

        if (changeDoctorOpinion !="yes" && changeDoctorOpinion!="no")
        {
            Console.WriteLine("Wrong command. Aborting...");
            return;
        }

        if (changeDoctorOpinion == "yes")
        {
            if (alternativeDoctors.Count == 0)
            {
                Console.WriteLine("No doctors found in the same specialty.");
                return;
            }

            for (int i=0; i<alternativeDoctors.Count; i++)
            {
                Console.WriteLine(i+" - "+alternativeDoctors[i].ToString());
            }

            int selectedDoctorIndex = -1;
            try
            {
                System.Console.Write("Please enter a number from the list: ");
                selectedDoctorIndex = ReadInt(0, alternativeDoctors.Count-1, "Number out of bounds!", "Number not recognized!");
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Aborting...");
                return;
            }

            newDoctor = alternativeDoctors[selectedDoctorIndex];
        }

        //change date?

        Console.WriteLine("Change date? Enter yes or no: ");

        string changeDateOpinion =ReadSanitizedLine().Trim().ToLower();

        if (changeDateOpinion !="yes" && changeDateOpinion!="no")
        {
            Console.WriteLine("Wrong command. Aborting...");
            return;
        }

        if (changeDateOpinion == "yes")
        {
            newDate = SelectDateAndTime();
            Console.WriteLine("You have selected the following date - "+ newDate);
        }

        //create checkup
        selectedCheckup.Doctor = new MongoDB.Driver.MongoDBRef("doctors", newDoctor.Id);
        DateTime oldDate = selectedCheckup.DateRange.Starts;
        selectedCheckup.DateRange = new DateRange(newDate, newDate.Add(Checkup.DefaultDuration), allowPast: false);
        
        
        if (!_hospital.AppointmentService.IsDoctorAvailable(selectedCheckup.DateRange, newDoctor))
        {
            Console.WriteLine("Checkup already taken.");
            return;
        }
        
        if (oldDate < _now.AddDays(2))
        {
            CheckupChangeRequest newRequest = new CheckupChangeRequest(
                selectedCheckup,
                CRUDOperation.UPDATE);
            Console.WriteLine("Checkup date is in less than 2 days from now. Change request sent.");
            _hospital.CheckupChangeRequestRepo.AddOrUpdate(newRequest);
        }
        else
        {
            _hospital.AppointmentService.UpsertCheckup(selectedCheckup);
            Console.WriteLine("Checkup updated.");
        }
        
        _hospital.PatientService.LogChange(CRUDOperation.UPDATE,_loggedInPatient);
        if (nextWillBlock)
        {
            _hospital.UserService.BlockUser(_user);
            throw new UserBlockedException("Updating too many checkups.");
        }

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
            iterationTime = iterationTime.Add(_checkupDuration);
            highestCheckupIndex += 1;
        }

        System.Console.Write("Please enter a number from the list: ");
        int selectedIndex = ReadInt(0, highestCheckupIndex-1, "Number out of bounds!", "Number not recognized!");

        inputDate = inputDate.AddHours(Globals.OpeningTime.Hour);
        inputDate = inputDate.Add(selectedIndex*_checkupDuration);

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

        if (DateTime.Compare(result.Date, _now.Date) == -1 )
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

        if (DateTime.Compare(result, _now) == -1 )
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
            selectedIndex = ReadInt(0, suitableDoctors.Count-1, "Number out of bounds!", "Number not recognized!");
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            return null;
        }

        return suitableDoctors[selectedIndex];
    }

    public void CreateCheckupAdvanced()
    {
        bool nextWillBlock = _hospital.PatientService.WillNextCRUDOperationBlock(CRUDOperation.CREATE, _loggedInPatient);
        if (nextWillBlock)
        {
            Console.WriteLine("Warning! Any additional checkup creation will result in account block!");
        }

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

        Doctor? selectedSuitableDoctor = SelectDoctor(selectedSpecialty);
        if (selectedSuitableDoctor == null)
        {
            return;
        }

        //TODO: this doesnt have to be in 15 minute slots
        System.Console.WriteLine("Please select starting time");
        DateTime intervalStart = SelectTime(new DateTime());
        System.Console.WriteLine("Please select starting time");
        DateTime intervalEnd = SelectTime(new DateTime());
        Console.Write("Please enter a date in dd-MM-yyyy format: ");
        DateTime deadline = SelectDate();

        if (intervalStart >= intervalEnd)
        {
            System.Console.WriteLine("Wrong start and end time. Aborting...");
            return;
        }

        List<Checkup> recommendedCheckups;

        Console.Write("Is time interval a priority? Enter y if yes, anything else for doctor: ");
        string choice = ReadSanitizedLine().Trim();
        bool isIntervalPriority = false;

        if (choice == "y")
        {
            isIntervalPriority = true;
        }

        DateRange interval = new DateRange(intervalStart,intervalEnd, true);
        recommendedCheckups = _hospital.AppointmentService.FindSuitableCheckups(selectedSuitableDoctor,interval,deadline,isIntervalPriority,_user);
        
        if (recommendedCheckups.Count == 1)
        {
            Checkup result = recommendedCheckups[0];
            Console.WriteLine("Recommendation:");
            Doctor referencedDoctor = _hospital.DoctorService.GetById((ObjectId)result.Doctor.Id);
            Console.WriteLine(referencedDoctor.ToString()+" "+result.DateRange.Starts);

            Console.Write("Create checkup? Enter y for yes: ");
            if (ReadSanitizedLine().Trim() == "y")
            {
                _hospital.AppointmentService.UpsertCheckup(result);
                Console.WriteLine("Checkup created.");
                
                _hospital.PatientService.LogChange(CRUDOperation.CREATE,_loggedInPatient);
                if (nextWillBlock)
                {
                    _hospital.UserService.BlockUser(_user);
                    throw new UserBlockedException("Creating too many checkups.");
                }

                return;
            }
            Console.WriteLine("Checkup creation canceled.");

        }
        else
        {
            Console.WriteLine("Recommendations:");
            for (int i=0; i<recommendedCheckups.Count; i++)
            {
                Checkup result = recommendedCheckups[i];
                Doctor referencedDoctor = _hospital.DoctorService.GetById((ObjectId)result.Doctor.Id);
                Console.WriteLine(i+" - "+referencedDoctor.ToString()+" "+result.DateRange.Starts);
            }

            System.Console.Write("Please enter a number from the list: ");
            int selectedIndex;
            try
            {
                selectedIndex = ReadInt(0, 2, "Number out of bounds!", "Number not recognized!");
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Aborting...");
                return;
            }
            _hospital.AppointmentService.UpsertCheckup(recommendedCheckups[selectedIndex]);
            Console.WriteLine("Checkup created.");
                
            _hospital.PatientService.LogChange(CRUDOperation.CREATE,_loggedInPatient);
            if (nextWillBlock)
            {
                _hospital.UserService.BlockUser(_user);
                throw new UserBlockedException("Creating too many checkups.");
            }
        }
    }

    public void CreateCheckup()
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

        Doctor? selectedDoctor = SelectDoctor(selectedSpecialty);
        if (selectedDoctor == null)
        {
            return;
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