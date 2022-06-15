using HospitalSystem.Core;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Globalization;
using HospitalSystem.Core.Medications.Requests;

namespace HospitalSystem.ConsoleUI;

public class DaysOffUI : HospitalClientUI
{

    public DaysOffUI(Hospital hospital) : base(hospital){}

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("INPUT OPTIONS:");
             System.Console.WriteLine("   1. View requests-(vr)" );
             System.Console.WriteLine("   2. Quit-(q)");
             System.Console.WriteLine("   3. Exit-(x)");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                if (choice == "view requests" || choice == "vr")
                {
                    ViewRequests();
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

    public void ViewRequests()
    {
        System.Console.Clear();
        var daysOffRequests = _hospital.DaysOffRequestService.GetAllOnPending().ToList();

        ShowDaysOffRequests(daysOffRequests);
        var request = EnterRequestNumber(daysOffRequests);
        var status = EnterStatus();
        var explanation = EnterExplenation(status);

        _hospital.DaysOffRequestService.UpdateStatus(request, status);
        _hospital.DaysOffRequestService.UpdateExplanation(request, explanation);
        

        System.Console.Write("Press anything to continue.");
        ReadSanitizedLine();
    }

    public void ShowDaysOffRequests(List<DaysOffRequest> daysOffRequests)
    {
        for(var i = 0; i < daysOffRequests.Count(); i++){
            System.Console.WriteLine(i + ".)");
            System.Console.WriteLine("Doctor: " + daysOffRequests[i].Doctor.FirstName + " " + daysOffRequests[i].Doctor.LastName + "(" + daysOffRequests[i].Doctor.Specialty + ")");
            System.Console.WriteLine("Date: " + daysOffRequests[i].DaysOff.Starts.ToString("dd/MM/yyyy") + " - " + daysOffRequests[i].DaysOff.Ends.Date.ToString("dd/MM/yyyy"));
            System.Console.WriteLine("Reason: (" + daysOffRequests[i].Reason + ")");
            System.Console.WriteLine("------------------------------------------");
        }
    }
    
    public DaysOffRequest EnterRequestNumber(List<DaysOffRequest> daysOffRequests)
    {
        System.Console.Write("Enter request number: ");
        var number = ReadInt();
        if(number < 0 || number >= daysOffRequests.Count())
        {
            throw new InvalidInputException("Number is out of range");
        }
        return daysOffRequests[number];
    }

    public RequestStatus EnterStatus()
    {
        System.Console.Write("Enter status(approved, denied): ");
        var requestStatus = ReadSanitizedLine();
        
        bool success = Enum.TryParse(requestStatus, true, out RequestStatus status);
        if (!success)
        {
            throw new InvalidInputException("Not a valid type.");
        }
        return status;
    }

    public string EnterExplenation(RequestStatus status)
    {
        var explenation = "";
        if(status == RequestStatus.DENIED)
        {
            System.Console.Write("Enter explenation: ");
            explenation = ReadSanitizedLine();
        }
        return explenation;
    }
}

