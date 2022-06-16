using HospitalSystem.Core;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Globalization;

namespace HospitalSystem.ConsoleUI;

public class CheckupUI : HospitalClientUI
{
    public CheckupUI(Hospital hospital) : base(hospital){}

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("INPUT OPTIONS:");
            System.Console.WriteLine("   1. View requests, change/delete-(vr)");
            System.Console.WriteLine("   2. Create checkup-(cr)");
            System.Console.WriteLine("   3. ");
            System.Console.WriteLine("   4. Quit-(q)");
            System.Console.WriteLine("   5. Exit-(x)");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                if (choice == "view requests" || choice == "vr")
                {
                    ViewRequests();
                }
                else if (choice == "create checkup" || choice == "cr")
                {
                    CreateCheckup();
                }
                else if (choice == "" || choice == "")
                {
                    continue;
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
                    System.Console.WriteLine("Invalid input - read the available commands!");
                    System.Console.Write("Intput anything to continue >> ");
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Intput anything to continue >> ");
            }
            catch (FormatException e)
            {
                System.Console.Write(e.Message + " Intput anything to continue >> ");
            }
            ReadSanitizedLine();
        }
    }

    public void ViewRequests()
    {
        System.Console.Clear();
        CheckupChangeRequestService cs = _hospital.CheckupChangeRequestService;
        List<CheckupChangeRequest> requests = cs.GetAll().ToList();
        requests.RemoveAll(u => u.RequestState != RequestState.PENDING);

        ShowRequests(requests);
    
        var indexId = EnterIndexId(requests);
        var state = EnterState(requests);

        System.Console.Write(requests[indexId].Checkup.DateRange.Starts.ToString());
        var requestState = ReadSanitizedLine();


        cs.UpdateRequest(requests[indexId], state);
        System.Console.Write("Successfully filled a request. Press anything to continue: ");
    }

    public void ShowRequests(List<CheckupChangeRequest> requests)
    {
        for(var i = 0; i < requests.Count; i++)
        {
            Patient pat = _hospital.PatientService.GetPatientById((ObjectId) requests[i].Checkup.Patient.Id);
            Doctor doc = _hospital.DoctorService.GetById((ObjectId) requests[i].Checkup.Doctor.Id);

            System.Console.WriteLine("Index ID: " + i);
            System.Console.WriteLine("Patient: " +  pat.FirstName + " " + pat.LastName);
            System.Console.WriteLine("Doctor: " +  doc.FirstName + " " + doc.LastName);
            System.Console.WriteLine("Start time: " + requests[i].Checkup.DateRange.Starts);
            System.Console.WriteLine("End time: " + requests[i].Checkup.DateRange.Ends);
            System.Console.WriteLine("--------------------------------------------------------------------");
            System.Console.WriteLine();
        }
    }
    
    public int EnterIndexId(List<CheckupChangeRequest> requests)
    {
        System.Console.Write("Enter index ID: ");
        int indexId = ReadInt(0, requests.Count() - 1);
        return indexId;
    }

    public RequestState EnterState(List<CheckupChangeRequest> requests)
    {
        System.Console.Write("Enter state(approved, denied): ");
        var requestState = ReadSanitizedLine();
        
        bool success = Enum.TryParse(requestState, true, out RequestState state);
        if (!success)
        {
            throw new InvalidInputException("Not a valid state.");
        }
        return state;
    }

    public void CreateCheckup(){

        Console.Clear();
        List<Patient> patients = _hospital.PatientService.GetPatients().ToList();

        var firstName = EnterPatientFirstName();
        var lastName = EnterPatientLastName();

        CheckIfPatientExist(patients, firstName, lastName);
        Patient patient = _hospital.PatientService.GetPatientByFullName(firstName, lastName);
        
        CheckIfReferralIsEmpty(patient);

        var referralPatient = patient.MedicalRecord.Referrals[0].Patient;
        var referralDoctor = patient.MedicalRecord.Referrals[0].Doctor;
        var referralAnamnesis =  patient.MedicalRecord.AnamnesisHistory;
        var anamnesis = CreateSentence(referralAnamnesis);
        var date = EnterDate();
        var busyAppointments = _hospital.AppointmentService.GetCheckupsByDay(date);
        busyAppointments.RemoveAll(c => c.Doctor.Id.ToString() != referralDoctor.Id.ToString());
        var allTimeSlots = GetAllTimeSlots(date);

        Console.Clear();

        RemoveBusyAppointments(busyAppointments, allTimeSlots);
        RemovePastAppointments(allTimeSlots);
        ShowFreeAppointments(allTimeSlots);

        var appointment = EnterAppointment(allTimeSlots);
        var dateTime = new DateTime(appointment.Year, appointment.Month, appointment.Day, appointment.Hour, appointment.Minute, appointment.Second);

        Checkup check = new Checkup(dateTime, referralPatient, referralDoctor, anamnesis);
        _hospital.AppointmentService.UpsertCheckup(check);

        System.Console.Write("Successfully created appointment. Press anything to continue.");
        ReadSanitizedLine();
    }

    public void CheckIfReferralIsEmpty(Patient patient)
    {
        if(patient.MedicalRecord.Referrals.Count() == 0){
            throw new InvalidInputException("Patient does not have referrals");
        }
    }
    
    public string EnterPatientFirstName(){
        System.Console.Write("Enter patient first name: ");
        string firstName = ReadSanitizedLine();
        return firstName;
    }

    public string EnterPatientLastName(){
        System.Console.Write("Enter patient last name: ");
        string lastName = ReadSanitizedLine();
        return lastName;
    }

    public void CheckIfPatientExist( List<Patient> patients, string firstName, string lastName){
        bool success = patients.Any(patient => patient.FirstName == firstName && patient.LastName == lastName);
        if(!success){
            throw new InvalidInputException("This patient does not exist");
        }
    }

    public string CreateSentence(List<string> referralAnamnesis)
    {
        string sentence = "";
        if(referralAnamnesis.Count != 0)
        {
            foreach(var anamnesis in referralAnamnesis)
            {
                sentence = sentence +  (char.ToUpper(anamnesis[0]) + anamnesis.Substring(1) + ", ");
            }
        }
        return sentence;
    }

    public DateTime EnterDate()
    {
        Console.Write("Enter date(dd-MM-yyyy): ");
        string date = ReadSanitizedLine();

        bool success = DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);

        if (!success) 
        {
            throw new InvalidInputException("Wrong date format.");  
        }

        if (DateTime.Compare(result.Date, DateTime.Now.Date) == -1 )
        {
            throw new InvalidInputException("The date is in the past. ");
        }
        return result;
    }

    public List<DateTime> GetAllTimeSlots(DateTime date)
    {
        DateTime iterationTime = HospitalSystem.Core.Utils.Globals.OpeningTime;
        List<DateTime> allApointments = new List<DateTime>();
        
        while (iterationTime.TimeOfDay != HospitalSystem.Core.Utils.Globals.ClosingTime.TimeOfDay)
        {
            DateTime dateTime = new DateTime(date.Year, date.Month, date.Day, iterationTime.Hour, iterationTime.Minute, iterationTime.Second);
            allApointments.Add(dateTime);
            iterationTime = iterationTime.Add(HospitalSystem.Core.Utils.Globals._checkupDuration);
        }
        return allApointments;
    }

    public void RemoveBusyAppointments(List<Checkup> busyAppointments, List<DateTime> allAppointments)
    {
        foreach(var busyAppointment in busyAppointments)
        {
            allAppointments.RemoveAll(appointment => appointment.TimeOfDay.ToString() == busyAppointment.DateRange.Starts.TimeOfDay.ToString());
        }
    }

    public void RemovePastAppointments(List<DateTime> allAppointments)
    {
            allAppointments.RemoveAll(appointment => appointment < DateTime.Now);
    }

    public void ShowFreeAppointments(List<DateTime> allAppointments)
    {
        for(var i = 0; i < allAppointments.Count(); i++)
        {
        Console.WriteLine(i + ". " + allAppointments[i].TimeOfDay);
        }
    }

    public DateTime EnterAppointment(List<DateTime> allAppointments)
    {
        System.Console.Write("Enter appointment number: ");
        var number = ReadInt(0, allAppointments.Count()-1);
        return allAppointments[number];
    }
}

