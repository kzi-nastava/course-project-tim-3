using System.Globalization;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core;
using HospitalSystem.Core.Surveys;

namespace HospitalSystem.ConsoleUI;

//TODO: CHANGE THIS
public class DoctorSearchUI : UserUI
{
    
    private Patient _loggedInPatient;

    public DoctorSearchUI(Hospital hospital, User user) : base(hospital, user) 
    {
        _loggedInPatient = _hospital.PatientService.GetPatientById((ObjectId) user.Person.Id);
    }

    public override void Start()
    {
        System.Console.WriteLine(@"
            Search options:
            n - search by name
            l - search by last name
            s - search by speciality
            ");

        Console.Write("Please enter a search option: ");
        string searchOption = ReadSanitizedLine().Trim();
        if (searchOption!= "n" && searchOption!= "l" && searchOption!= "s")
        {
            Console.WriteLine("Wrong option entered. Aborting...");
        }

        Console.Write("Please enter a search keyword: ");
        string keyword = ReadSanitizedLine().Trim();

        List<Doctor> filteredDoctors = new List<Doctor>();

        switch (searchOption)
        {
            case "n":
                filteredDoctors = _hospital.DoctorService.GetManyByName(keyword);
                break;
            case "l":
                filteredDoctors = _hospital.DoctorService.GetManyByLastName(keyword);
                break;
            case "s":
                filteredDoctors = _hospital.DoctorService.GetManyBySpecialty(keyword);
                break;
        }

        if (filteredDoctors.Count == 0)
        {
            Console.WriteLine("No doctors found.");
            return;
        }

        Console.WriteLine(filteredDoctors.Count + " doctors found.");
        System.Console.WriteLine(@"
            Sort options:
            n - sort by name
            l - sort by last name
            s - sort by specialty
            a - sort by average rating
            ");
        
        string sortOption = ReadSanitizedLine().Trim();
        if (sortOption == "n")
        {
            filteredDoctors.Sort((doctor1, doctor2)=> String.Compare(doctor1.FirstName, doctor2.FirstName));
        }
        else if (sortOption == "l")
        {
            filteredDoctors.Sort((doctor1, doctor2)=> String.Compare(doctor1.LastName, doctor2.LastName));
        }
        else if (sortOption == "s")
        {
            filteredDoctors.Sort((doctor1, doctor2)=> String.Compare(doctor1.Specialty.ToString(), doctor2.Specialty.ToString()));
        }
         else if (sortOption == "a")
        {
            filteredDoctors.Sort((doctor1, doctor2)=>  _hospital.DoctorSurveyService.GetAverageRating(doctor1).CompareTo(_hospital.DoctorSurveyService.GetAverageRating(doctor2)));
        }

        for (int i=0; i<filteredDoctors.Count; i++)
        {
            string rating = "no rating";
            double averageRating = _hospital.DoctorSurveyService.GetAverageRating(filteredDoctors[i]);
            if (averageRating != 10){
                rating = averageRating + "/5";
            }
            Console.WriteLine(i+" - "+filteredDoctors[i].ToString() + " " + rating);
        }

        System.Console.Write("Create checkup? Enter y to continue, anything else to return: ");
        string continuteOpinion = ReadSanitizedLine().Trim().ToLower();
        if (continuteOpinion != "y"){
            return;
        }

        System.Console.Write("Please enter a number from the list: ");
        int selectedIndex;
        try
        {
            selectedIndex = ReadInt(0, filteredDoctors.Count-1);
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            return;
        }

        CreateCheckup(filteredDoctors[selectedIndex]);
        
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


