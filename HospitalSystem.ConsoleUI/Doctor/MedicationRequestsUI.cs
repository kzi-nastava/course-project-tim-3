using MongoDB.Bson;
using MongoDB.Driver;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

public class MedicationRequestsUI : UserUI
{
    public MedicationRequestsUI(Hospital hospital, User user) : base(hospital, user) { }
    public override void Start()
    {
        Console.Clear();
        List<MedicationRequest> requested = _hospital.MedicationRequestService.GetSent().ToList();
        PrintMedicationRequests(requested);
        Console.Write("\nOptions:\n1. Review request\n2. Back\n");
        Console.Write(">>");
        string? input = Console.ReadLine();
        switch (input)
        {
            case "1":
            {
                ReviewMedicationRequests(requested);
                break;
            }
            case "2":
                break;
            default:
                Console.WriteLine("Wrong input. Please choose a valid option.");
                break;
        }
    }

    public void PrintMedicationRequests(List<MedicationRequest> requested)
    {
        int i = 1;
        Console.WriteLine(String.Format("\n{0,5} {1,24} {2,25} {3,20}", "Nr.", "Medication name", "Director comment", "Date of creation"));
        foreach (MedicationRequest request in requested)
        {
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 60)));
            Console.WriteLine(String.Format("{0,5} {1,24} {2,20} {3,30}", i, request.Requested.Name, request.DirectorComment, request.Created));
            i++;
        }
    }

    public void ReviewMedicationRequests(List<MedicationRequest> requested)
    {
        bool back = false;
        while (!back)
        {
            Console.WriteLine("\nEnter request number");
            Console.Write(">>");
            var isNumber = int.TryParse(Console.ReadLine(), out int requestNumber);
            if (isNumber == true && requestNumber > 0 && requestNumber <= requested.Count())
            {
                back = ReviewMedicationMenu(requested, requestNumber);
            }
            else
            {
                Console.WriteLine("Wrong input. Please enter a valid option.");
            }
        }
    }

    public bool ReviewMedicationMenu(List<MedicationRequest> requested, int requestNumber)
    {
        while (true)
        {
            MedicationRequest request = requested[requestNumber-1];
            Console.Write("\n" + request);
            Console.WriteLine("\n1. Approve\n2. Deny\n3. Back");
            Console.Write(">> ");
            string? option = Console.ReadLine();
            switch (option)
            {
                case "1":
                {
                    _hospital.MedicationRequestService.Approve(request);
                    Console.WriteLine("Request approved.");
                    return true;
                }
                case "2":
                {
                    Console.Write("\nWrite comment >>");
                    string? comment = Console.ReadLine();
                    if (comment != null)
                        request.DoctorComment = comment;
                    _hospital.MedicationRequestService.Deny(request);
                    Console.WriteLine("\nRequest denied.");
                    return true;
                }
                case "3":
                {
                    return true;
                }
                default:
                {
                    Console.WriteLine("Wrong input. Please enter a valid option.");
                    break;
                }
            }
        }
    }
}