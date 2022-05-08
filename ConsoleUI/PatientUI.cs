namespace Hospital;
using System.Globalization;
using MongoDB.Bson;

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
            selectedIndex = ReadInt(0, checkups.Count-1, "Please enter a number from the list: ","Number out of bounds!","Number not recognized!");
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
        Checkup selectedCheckup;
        try
        {
            selectedCheckup = SelectCheckup();
        }
        catch (QuitToMainMenuException)
        {
            return;
        }

        _hospital.AppointmentRepo.DeleteCheckup(selectedCheckup);
        Console.WriteLine("Checkup deleted.");

    }

    public void UpdateCheckup(){
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
                selectedDoctorIndex = ReadInt(0, alternativeDoctors.Count-1, "Please enter a number from the list: ","Number out of bounds!","Number not recognized!");
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
        selectedCheckup.TimeAndDate = newDate;
        
        if (_hospital.AppointmentRepo.IsDoctorBusy((DateTime)newDate,newDoctor))
        {
            Console.WriteLine("Checkup already taken.");
            return;
        }

        _hospital.AppointmentRepo.AddOrUpdateCheckup(selectedCheckup);
        Console.WriteLine("Checkup updated.");
    }

    public string ConvertAppointmentToString(Appointment a)
    {
        string output = "";

        output += a.TimeAndDate +" ";
        Doctor doctor = _hospital.DoctorRepo.GetDoctorById((ObjectId)a.Doctor.Id);
        output += doctor.FirstName+" "+doctor.LastName;

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
    public DateTime SelectDateAndTime ()
    {   
        DateTime result;

        // date selection
        Console.Write("Please enter a date in dd-MM-yyyy format: ");
        string inputDate = ReadSanitizedLine().Trim();

        bool success = DateTime.TryParseExact(inputDate, 
                       "dd-MM-yyyy", 
                       CultureInfo.InvariantCulture, 
                       DateTimeStyles.None, 
                       out result);

        if (!success) 
        {
            throw new InvalidInputException("Wrong date entered.");  
        }

        if (DateTime.Compare(result.Date, _now.Date) == -1 )
        {
            throw new InvalidInputException("The date entered is in past.");
        }
        
        // time selection

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

        int selectedIndex = ReadInt(0, highestCheckupIndex, "Please enter a number from the list: ","Number out of bounds!","Number not recognized!");

        result = result.AddHours(_openingTime.Hour);
        result = result.Add(selectedIndex*_checkupDuration);
       
        //TODO: The listed times shouldnt be the ones that expired

        if (DateTime.Compare(result, _now) == -1 )
        {
             throw new InvalidInputException("Selected date and time expired.");
        } 

        return result;
    }

    public void CreateCheckup()
    {
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

        List<Doctor> suitableDoctors =  _hospital.DoctorRepo.GetDoctorBySpecialty(selectedSpecialty);

        if (suitableDoctors.Count == 0)
        {
            Console.WriteLine("No doctors found in selected specialty.");
            ReadSanitizedLine();
            return;
        }

        for (int i=0; i<suitableDoctors.Count; i++)
        {
            Console.WriteLine(i+" - "+suitableDoctors[i].ToString());
        }

        int selectedIndex = -1;
        try
        {
            selectedIndex = ReadInt(0, suitableDoctors.Count-1, "Please enter a number from the list: ","Number out of bounds!","Number not recognized!");
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            return;
        }

        Doctor selectedSuitableDoctor = suitableDoctors[selectedIndex];

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
        
        this._hospital.AppointmentRepo.AddOrUpdateCheckup(newCheckup);
        Console.WriteLine("Checkup created");
        
    }

    public void ManageAppointments()
    {
        while (true){
            System.Console.WriteLine(@"
            Commands:
            cc - create checkup
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
            catch (Exception e)
            {
                System.Console.Write(e.Message);
            }
        }
    }

    public override void Start()
    {

        while (true){
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}