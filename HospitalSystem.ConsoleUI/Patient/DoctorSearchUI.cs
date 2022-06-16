using MongoDB.Bson;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

//TODO: CHANGE THIS
public class DoctorSearchUI : PatientUI
{

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
            return;
        }

        List<Doctor> doctors = SearchDoctors(searchOption);
        if (doctors.Count == 0)
        {
            Console.WriteLine("No doctors found.");
            return;
        }

        Console.WriteLine(doctors.Count + " doctors found.");
        System.Console.WriteLine(@"
            Sort options:
            n - sort by name
            l - sort by last name
            s - sort by specialty
            a - sort by average rating
            ");
        
        string sortOption = ReadSanitizedLine().Trim();
        doctors = SortDoctors(doctors,sortOption);

        PrintDoctors(doctors);

        System.Console.Write("Create checkup? Enter y to continue, anything else to return: ");
        string continuteOpinion = ReadSanitizedLine().Trim().ToLower();
        if (continuteOpinion != "y"){
            return;
        }

        System.Console.Write("Please enter a number from the list: ");
        int selectedIndex;
        try
        {
            selectedIndex = ReadInt(0, doctors.Count-1);
        }
        catch (InvalidInputException e)
        {
            System.Console.Write(e.Message + " Aborting...");
            return;
        }

        CreateCheckup(doctors[selectedIndex]);
        
    }

    public List<Doctor> SearchDoctors (string searchOption)
    {
        Console.Write("Please enter a search keyword: ");
        string keyword = ReadSanitizedLine().Trim();

        List<Doctor> doctors = new List<Doctor>();

        switch (searchOption)
        {
            case "n":
                doctors = _hospital.DoctorService.GetManyByName(keyword);
                break;
            case "l":
                doctors = _hospital.DoctorService.GetManyByLastName(keyword);
                break;
            case "s":
                doctors = _hospital.DoctorService.GetManyBySpecialty(keyword);
                break;
        }
        return doctors;
    }

    public List<Doctor> SortDoctors (List<Doctor> doctors, string sortOption)
    {
        if (sortOption == "n")
        {
            doctors.Sort((doctor1, doctor2)=> String.Compare(doctor1.FirstName, doctor2.FirstName));
        }
        else if (sortOption == "l")
        {
            doctors.Sort((doctor1, doctor2)=> String.Compare(doctor1.LastName, doctor2.LastName));
        }
        else if (sortOption == "s")
        {
            doctors.Sort((doctor1, doctor2)=> String.Compare(doctor1.Specialty.ToString(), doctor2.Specialty.ToString()));
        }
         else if (sortOption == "a")
        {
            doctors.Sort((doctor1, doctor2)=>  _hospital.DoctorSurveyService.GetAverageRating(doctor1).CompareTo(_hospital.DoctorSurveyService.GetAverageRating(doctor2)));
        }
        return doctors;
    }

    public void PrintDoctors(List<Doctor> doctors)
    {
        for (int i=0; i<doctors.Count; i++)
        {
            string rating = "no rating";
            double averageRating = _hospital.DoctorSurveyService.GetAverageRating(doctors[i]);
            if (averageRating != 10){
                rating = averageRating.ToString("0.##") + "/5";
            }
            Console.WriteLine(i+" - "+doctors[i].ToString() + " " + rating);
        }
    }
}