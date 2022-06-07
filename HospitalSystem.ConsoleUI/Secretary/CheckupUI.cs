using HospitalSystem.Core;
using MongoDB.Bson;

namespace HospitalSystem.ConsoleUI;

public class CheckupUI : ConsoleUI
{

    public CheckupUI(Hospital hospital) : base(hospital){}

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("INPUT OPTIONS:");
             System.Console.WriteLine("   1. View requests, change/delete-(vr)");
             System.Console.WriteLine("   2. ???");
             System.Console.WriteLine("   3. ????");
             System.Console.WriteLine("   4. Quit-(q)");
             System.Console.WriteLine("   5. Exit-(x)");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                if (choice == "view requests" || choice == "vr")
                {
                    CheckRequests();
                }
                else if (choice == "???" || choice == "?")
                {
                    continue;
                }
                else if (choice == "????" || choice == "??")
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
                    ReadSanitizedLine();
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Intput anything to continue >> ");
                ReadSanitizedLine();
            }
            catch (FormatException e)
            {
                System.Console.Write(e.Message + " Intput anything to continue >> ");
                ReadSanitizedLine();
            }
        }
    }

    public void CheckRequests()
    {
        System.Console.Clear();
        CheckupChangeRequestRepository cr = _hospital.CheckupChangeRequestRepo;
        List<CheckupChangeRequest> requests = _hospital.CheckupChangeRequestRepo.GetAll().ToList();
        requests.RemoveAll(u => u.RequestState != RequestState.PENDING);
        
        for(var i = 0; i < requests.Count; i++){
            Patient pat = _hospital.PatientRepo.GetPatientById((ObjectId) requests[i].Checkup.Patient.Id);
            Doctor doc = _hospital.DoctorRepo.GetById((ObjectId) requests[i].Checkup.Doctor.Id);

            System.Console.WriteLine("Index ID: " + i);
            System.Console.WriteLine("ID: " + requests[i].Id.ToString());
            System.Console.WriteLine("Patient: " +  pat.FirstName + " " + pat.LastName);
            System.Console.WriteLine("Doctor: " +  doc.FirstName + " " + doc.LastName);
            System.Console.WriteLine("Start time: " + requests[i].Checkup.DateRange.Starts);
            System.Console.WriteLine("End time: " + requests[i].Checkup.DateRange.Ends);
            System.Console.WriteLine("RequestState: " + requests[i].RequestState);
            System.Console.WriteLine("--------------------------------------------------------------------");
            System.Console.WriteLine();
        }
        
        System.Console.Write("Enter id: ");
        int indexId = ReadInt();

        System.Console.Write("Enter state(approved, denied): ");
        string stringState = ReadSanitizedLine();
        
        if(stringState != "approved" || stringState != "denied")
        {
             throw new InvalidInputException("Invalid input!");
        }
 
        if (stringState == "approved")
        {
            cr.UpdatePendingRequest(indexId, RequestState.APPROVED);
            System.Console.Write("Successfully approved request. Press anything to continue: ");
            ReadSanitizedLine();
        }
        else if(stringState == "denied")
        {
            cr.UpdatePendingRequest(indexId, RequestState.DENIED);
            System.Console.Write("Successfully denied request. Press anything to continue: ");
            ReadSanitizedLine();
        }
        else
        {
            System.Console.Write("Successfully denied request. Press anything to continue: ");
            ReadSanitizedLine();
        }
    }
}