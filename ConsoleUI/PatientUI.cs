namespace Hospital;
using System.Globalization;
using MongoDB.Bson;

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

public class PatientUI : ConsoleUI
{
    //there might be a better way to set opening time, only time will be used
    //those times should be stored somewhere else
    private DateTime _openingTime = new DateTime(2000, 10, 20, 9, 0, 0);
    private DateTime _closingTime = new DateTime(2000, 10, 20, 17, 0, 0);
    private DateTime _now = DateTime.Now;
    private TimeSpan _checkupDuration = new TimeSpan(0,0,15,0);
    private Patient _loggedInPatient;

    public PatientUI(Hospital _hospital, User? _user) : base(_hospital) 
    {
        this._user = _user;
        _loggedInPatient = _hospital.PatientRepo.GetPatientById((ObjectId) _user.Person.Id);
    }

    public bool WillNextCRUDOperationBlock(CRUDOperation crudOperation)
    {
        int limit;
        //TODO: unhardcode this
        switch (crudOperation)
        {
            case CRUDOperation.CREATE:
                limit = 8;
                break;
            case CRUDOperation.UPDATE:
                limit = 4;
                break;
            case CRUDOperation.DELETE:
                limit = 4;
                break;
            default:
                //this is dummy value, as of now there are no read restrictions
                limit = 999;
                break;
        }

        int count = 0;
        foreach (CheckupChangeLog log in _loggedInPatient.CheckupChangeLogs)
        {
            if (log.TimeAndDate > _now.AddDays(-30) &&  log.CRUDOperation == crudOperation)
            {
                count++;
            }
        }

        if (count+1 > limit)
        {
            return true;
        }
        return false;
    }

    public void LogChange(CRUDOperation crudOperation)
    {
        CheckupChangeLog log = new CheckupChangeLog(DateTime.Now,crudOperation);
        _loggedInPatient.CheckupChangeLogs.Add(log);
        _hospital.PatientRepo.AddOrUpdatePatient(_loggedInPatient);
    }

    public Checkup SelectCheckup ()
    {
        ShowCheckups();
        List<Checkup> checkups = _hospital.AppointmentRepo.GetCheckupsByPatient(_loggedInPatient.Id);
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
        bool nextWillBlock = WillNextCRUDOperationBlock(CRUDOperation.DELETE);
        if (nextWillBlock)
        {
            Console.WriteLine("Warning! Any additional checkup deletion will result in account block!");
        }
        Checkup selectedCheckup;
        try
        {
            selectedCheckup = SelectCheckup();
        }
        catch (QuitToMainMenuException)
        {
            return;
        }

        if (selectedCheckup.TimeAndDate < _now.AddDays(2))
        {
            CheckupChangeRequest newRequest = new CheckupChangeRequest(
                selectedCheckup,
                CRUDOperation.DELETE);
                Console.WriteLine("Checkup date is in less than 2 days from now. Change request sent.");
                _hospital.CheckupChangeRequestRepo.AddOrUpdateCheckupChangeRequest(newRequest);
        }
        else
        {
            _hospital.AppointmentRepo.DeleteCheckup(selectedCheckup);
            Console.WriteLine("Checkup deleted.");
        }

        LogChange(CRUDOperation.DELETE);
        if (nextWillBlock)
        {
            _user.BlockStatus = Block.BY_SYSTEM;
            _hospital.UserRepo.AddOrUpdateUser(_user);
            throw new UserBlockedException("Deleting too many checkups.");
        }

    }

    public void UpdateCheckup(){

        bool nextWillBlock = WillNextCRUDOperationBlock(CRUDOperation.UPDATE);
        if (nextWillBlock)
        {
            Console.WriteLine("Warning! Any additional checkup updating will result in account block!");
        }
        Checkup selectedCheckup;
        try
        {
            selectedCheckup = SelectCheckup();
        }
        catch (QuitToMainMenuException)
        {
            return;
        }
        Console.WriteLine ("You have selected " + ConvertAppointmentToString(selectedCheckup));

        Doctor currentDoctor = _hospital.DoctorRepo.GetDoctorById((ObjectId)selectedCheckup.Doctor.Id);
        DateTime existingDate = selectedCheckup.TimeAndDate;
        
        List<Doctor> alternativeDoctors =  _hospital.DoctorRepo.GetDoctorBySpecialty(currentDoctor.Specialty);
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
        DateTime oldDate = selectedCheckup.TimeAndDate;
        selectedCheckup.TimeAndDate = newDate;
        //TODO: if both change doctor and change date are false, dont create a checkup

        if (_hospital.AppointmentRepo.IsDoctorBusy((DateTime)newDate,newDoctor))
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
                _hospital.CheckupChangeRequestRepo.AddOrUpdateCheckupChangeRequest(newRequest);
        }
        else
        {
            _hospital.AppointmentRepo.AddOrUpdateCheckup(selectedCheckup);
            Console.WriteLine("Checkup updated.");
        }
        
        LogChange(CRUDOperation.UPDATE);
        if (nextWillBlock)
        {
            _user.BlockStatus = Block.BY_SYSTEM;
            _hospital.UserRepo.AddOrUpdateUser(_user);
            throw new UserBlockedException("Updating too many checkups.");
        }

    }

    public string ConvertAppointmentToString(Appointment a)
    {
        string output = "";

        output += a.TimeAndDate +" ";
        Doctor doctor = _hospital.DoctorRepo.GetDoctorById((ObjectId)a.Doctor.Id);
        output += doctor.ToString();

        return output;
    }


    public void ShowCheckups()
    {
        List<Checkup> checkups = _hospital.AppointmentRepo.GetCheckupsByPatient(_loggedInPatient.Id);
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

    public void showOperations()
    {
        List<Operation> operations = _hospital.AppointmentRepo.GetOperationsByPatient(_loggedInPatient.Id);
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
        ShowCheckups();
        Console.WriteLine("### Operations ###");
        showOperations();

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
            case "DERMATOLOGY":
                return Specialty.DERMATOLOGY;
            case "RADIOLOGY":
                return Specialty.RADIOLOGY;
            case "STOMATOLOGY":
                return Specialty.STOMATOLOGY;
            case "OPHTHALMOLOGY":
                return Specialty.OPHTHALMOLOGY;
            case "FAMILY_MEDICINE":
                return Specialty.FAMILY_MEDICINE;
            default:
                throw new InvalidInputException("Speciality not recognized.");
        }
    }

    //takes a datetime with date part already set, and sets its time part
    public DateTime SelectTime(DateTime inputDate)
    {
        int highestCheckupIndex = 0;
        DateTime iterationTime = _openingTime;
        
        while (iterationTime.TimeOfDay != _closingTime.TimeOfDay)
        {
            Console.WriteLine(highestCheckupIndex + " - " + iterationTime.ToString("HH:mm"));
            iterationTime = iterationTime.Add(_checkupDuration);
            highestCheckupIndex += 1;
        }

        //while loop will add an extra "1" at the end of the loop, we will remove that
        highestCheckupIndex -= 1;

        System.Console.Write("Please enter a number from the list: ");
        int selectedIndex = ReadInt(0, highestCheckupIndex, "Number out of bounds!", "Number not recognized!");

        inputDate = inputDate.AddHours(_openingTime.Hour);
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
        List<Doctor> suitableDoctors =  _hospital.DoctorRepo.GetDoctorBySpecialty(selectedSpecialty);

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

    public List<Checkup> GetFirstFewFreeCheckups(DateTime intervalStart, DateTime intervalEnd, Specialty speciality, int numberOfCheckups)
    {
        List<Checkup> checkups = new List<Checkup>();
        DateTime iterationDate = RoundUp(DateTime.Now,TimeSpan.FromMinutes(15));

        while ( checkups.Count < numberOfCheckups)
        {
            if (iterationDate.TimeOfDay >=_closingTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day, _openingTime.Hour, _openingTime.Minute, _openingTime.Second);
                iterationDate = iterationDate.AddDays(1);
                continue;
            }

            if (iterationDate.TimeOfDay < _openingTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day, _openingTime.Hour, _openingTime.Minute, _openingTime.Second);
                continue;
            }

            if (!(intervalStart.TimeOfDay<=iterationDate.TimeOfDay && iterationDate.TimeOfDay<intervalEnd.TimeOfDay))
            {
                iterationDate = iterationDate.AddMinutes(15);
                continue;
            }

            foreach (Doctor doctor in _hospital.DoctorRepo.GetDoctorBySpecialty(speciality))
            {
                if (_hospital.AppointmentRepo.IsDoctorBusy(iterationDate,doctor))
                {
                    continue;
                }
                else
                {
                    if (checkups.Count >= numberOfCheckups)
                    {
                        break;
                    }

                    Checkup newCheckup = new Checkup(
                    iterationDate,
                    new MongoDB.Driver.MongoDBRef("patients", _user.Person.Id),
                    new MongoDB.Driver.MongoDBRef("doctors", doctor.Id),
                    "no anamnesis");
                    checkups.Add(newCheckup);
                }
            }
            iterationDate = iterationDate.AddMinutes(15);
        }
        return checkups;
    }

    public List<Checkup> GetFirstFewFreeCheckups(Doctor doctor, int numberOfCheckups)
    {
        List<Checkup> checkups = new List<Checkup>();
        DateTime iterationDate = RoundUp(DateTime.Now,TimeSpan.FromMinutes(15));

        while ( checkups.Count < numberOfCheckups)
        {
            if (iterationDate.TimeOfDay >=_closingTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day, _openingTime.Hour, _openingTime.Minute, _openingTime.Second);
                iterationDate = iterationDate.AddDays(1);
                continue;
            }

            if (iterationDate.TimeOfDay < _openingTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day, _openingTime.Hour, _openingTime.Minute, _openingTime.Second);
                continue;
            }
            
            if (_hospital.AppointmentRepo.IsDoctorBusy(iterationDate,doctor))
            {
                iterationDate = iterationDate.AddMinutes(15);
                continue;
            }
            else
            {
                Checkup newCheckup = new Checkup(
                iterationDate,
                new MongoDB.Driver.MongoDBRef("patients", _user.Person.Id),
                new MongoDB.Driver.MongoDBRef("doctors", doctor.Id),
                "no anamnesis");
                checkups.Add(newCheckup);
                iterationDate = iterationDate.AddMinutes(15);
            }
        }
        return checkups;
    }

    public DateTime RoundUp(DateTime dt, TimeSpan d)
    {
        return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
    }

    public List<Checkup> FindSuitableCheckups(Doctor doctor, DateTime intervalStart, DateTime intervalEnd, DateTime deadline, bool isIntervalPriority)
    {
        List<Checkup> checkups = new List<Checkup>();
        DateTime iterationDate = RoundUp(DateTime.Now,TimeSpan.FromMinutes(15));
        while ( iterationDate < deadline)
        {
            if (iterationDate.TimeOfDay >=_closingTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day, _openingTime.Hour, _openingTime.Minute, _openingTime.Second);
                iterationDate = iterationDate.AddDays(1);
                continue;
            }

            if (iterationDate.TimeOfDay < _openingTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day, _openingTime.Hour, _openingTime.Minute, _openingTime.Second);
                continue;
            }

            if (!(intervalStart.TimeOfDay<=iterationDate.TimeOfDay && iterationDate.TimeOfDay<intervalEnd.TimeOfDay))
            {
                iterationDate = iterationDate.AddMinutes(15);
                continue;
            }
            
            if (_hospital.AppointmentRepo.IsDoctorBusy(iterationDate,doctor))
            {
                iterationDate = iterationDate.AddMinutes(15);
                continue;
            }
            else
            {
                Checkup newCheckup = new Checkup(
                    iterationDate,
                    new MongoDB.Driver.MongoDBRef("patients", _user.Person.Id),
                    new MongoDB.Driver.MongoDBRef("doctors", doctor.Id),
                    "no anamnesis");
                checkups.Add(newCheckup);
                return checkups;
            }
        }
        //if code gets to this point, it means it hasnt found a good match
        if (isIntervalPriority)
        {
            return GetFirstFewFreeCheckups(intervalStart,intervalEnd,doctor.Specialty,3);
        }
        return GetFirstFewFreeCheckups(doctor,3);
    }

    public List<Checkup> FindCheckupsPriorityInterval(Doctor doctor, DateTime intervalStart, DateTime intervalEnd, DateTime deadline)
    {
        List<Checkup> checkups = new List<Checkup>();
        return checkups;
    }

    public void CreateCheckupAdvanced()
    {
        bool nextWillBlock = WillNextCRUDOperationBlock(CRUDOperation.CREATE);
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

        List<Checkup> recommendationResults;

        Console.Write("Is time interval a priority? Enter y if yes, anything else for doctor: ");
        string choice = ReadSanitizedLine().Trim();
        bool isIntervalPriority = false;

        if (choice == "y")
        {
            isIntervalPriority = true;
        }

        recommendationResults = FindSuitableCheckups(selectedSuitableDoctor,intervalStart,intervalEnd,deadline,isIntervalPriority);
        
        if (recommendationResults.Count == 1)
        {
            Checkup result = recommendationResults[0];
            Console.WriteLine("Recommendation:");
            Doctor referencedDoctor = _hospital.DoctorRepo.GetDoctorById((ObjectId)result.Doctor.Id);
            Console.WriteLine(referencedDoctor.ToString()+" "+result.TimeAndDate);

            Console.Write("Create checkup? Enter y for yes: ");
            if (ReadSanitizedLine().Trim() == "y")
            {
                _hospital.AppointmentRepo.AddOrUpdateCheckup(result);
                Console.WriteLine("Checkup created.");
                
                LogChange(CRUDOperation.CREATE);
                if (nextWillBlock)
                {
                    _user.BlockStatus = Block.BY_SYSTEM;
                    _hospital.UserRepo.AddOrUpdateUser(_user);
                    throw new UserBlockedException("Creating too many checkups.");
                }

                return;
            }
            Console.WriteLine("Checkup creation canceled.");

        }
        else
        {
            Console.WriteLine("Recommendations:");
            for (int i=0; i<recommendationResults.Count; i++)
            {
                Checkup result = recommendationResults[i];
                Doctor referencedDoctor = _hospital.DoctorRepo.GetDoctorById((ObjectId)result.Doctor.Id);
                Console.WriteLine(i+" - "+referencedDoctor.ToString()+" "+result.TimeAndDate);
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
            _hospital.AppointmentRepo.AddOrUpdateCheckup(recommendationResults[selectedIndex]);
            Console.WriteLine("Checkup created.");
                
            LogChange(CRUDOperation.CREATE);
            if (nextWillBlock)
            {
                _user.BlockStatus = Block.BY_SYSTEM;
                _hospital.UserRepo.AddOrUpdateUser(_user);
                throw new UserBlockedException("Creating too many checkups.");
            }

        }

    }

    public void CreateCheckup()
    {

        //TODO: change this
        bool nextWillBlock = WillNextCRUDOperationBlock(CRUDOperation.CREATE);
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

        Doctor? selectedSuitableDoctor = SelectDoctor(selectedSpecialty);
        if (selectedSuitableDoctor == null)
        {
            return;
        }

        if (_hospital.AppointmentRepo.IsDoctorBusy(selectedDate,selectedSuitableDoctor))
        {
            Console.WriteLine("Checkup already taken.");
            return;
        }
        Console.WriteLine("Checkup is free to schedule");
        
        //TODO: Might want to create an additional expiry check for checkup timedate
        Checkup newCheckup = new Checkup(
            selectedDate,
            new MongoDB.Driver.MongoDBRef("patients", _user.Person.Id),
            new MongoDB.Driver.MongoDBRef("doctors", selectedSuitableDoctor.Id),
            "no anamnesis");
        
        _hospital.AppointmentRepo.AddOrUpdateCheckup(newCheckup);
        Console.WriteLine("Checkup created");
        
        LogChange(CRUDOperation.CREATE);
        if (nextWillBlock)
        {
            _user.BlockStatus = Block.BY_SYSTEM;
            _hospital.UserRepo.AddOrUpdateUser(_user);
            throw new UserBlockedException("Creating too many checkups.");
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
            exit - quit the program

            ");
            string selectedOption = ReadSanitizedLine().Trim();
            
            try
            {
                if (selectedOption == "ma")
                {
                    ManageAppointments();
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
}