using MongoDB.Bson;

namespace Hospital;

public class DoctorUI : ConsoleUI
{
    public DoctorUI(Hospital _hospital) : base(_hospital) {}

     public override void Start()
    {
        while (true)
        {
            Console.WriteLine("\nChoose an option below:\n\n1. View appointments for a specific day\n2. View timetable\n3. Exit");
            Console.Write("\n>>");
            var option = Console.ReadLine().Trim();
            switch (option)
            {
                case "1":
                {
                    ShowCheckupsByDay();
                    break;
                }
                case "2":
                {
                    ShowNextThreeDays();
                    break;
                }
                case "3":
                {
                    break;
                }
            }
        }
        
    }

    public void ShowCheckupsByDay()
    {
        Console.Write("\nEnter date (dd.mm.yyyy) >> ");
        var date = Console.ReadLine();
        List<Checkup> checkups = _hospital.AppointmentRepo.GetCheckupsByDay(Convert.ToDateTime(date));
        PrintCheckups(checkups);
    }

    public void ShowNextThreeDays()
    {
        Console.WriteLine("\nThese are your checkups for the next 3 days:\n");
        List<Checkup> checkups = _hospital.AppointmentRepo.GetCheckupsByDay(DateTime.Now);
        checkups.AddRange(_hospital.AppointmentRepo.GetCheckupsByDay(DateTime.Today.AddDays(1)));
        checkups.AddRange(_hospital.AppointmentRepo.GetCheckupsByDay(DateTime.Today.AddDays(2)));
        PrintCheckups(checkups);
        while (true)
        {
            Console.Write("\nOptions:\n\n1. See patient info for checkup\n2.Start checkup\n3. Back\n\n");
            Console.Write(">>");
            var option = Console.ReadLine().Trim();
            switch (option)
            {
                case "1":
                {
                    Console.Write("\nEnter checkup number >> ");
                    try
                    {
                        var checkupNumber = int.Parse(Console.ReadLine());
                        ShowPatientInfo(checkups[checkupNumber-1]);

                    } catch (IOException e)
                    {
                        Console.WriteLine("Wrong input.");
                    } catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("Wrong input.");
                    } catch (NullReferenceException e)
                    {
                        Console.WriteLine("Wrong input.");
                    }
                    break;
                }
                case "2":
                {
                    ShowNextThreeDays();
                    break;
                }
                case "3":
                {
                    break;
                }
            }
        }
    }

    public void PrintCheckups(List<Checkup> checkups)
    {
        Console.WriteLine(String.Format("{0,5} {1,12} {2,12} {3,25}", "Nr.", "Date", "Time", "Patient"));
        int i = 1;
        foreach (Checkup checkup in checkups)
        {
            Patient patient = _hospital.PatientRepo.GetPatientById((ObjectId)checkup.Patient.Id);
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 60)));
            Console.WriteLine(String.Format("{0,5} {1,12} {2,12} {3,25}", i, checkup.TimeAndDate.ToString("dd.MM.yyyy"), 
            checkup.TimeAndDate.ToString("HH:mm"), patient));
            i++;
        }
    }

    public void ShowPatientInfo(Checkup checkup)
    {
        
        Patient patient = _hospital.PatientRepo.GetPatientById((ObjectId)checkup.Patient.Id);
        Console.Write(patient.MedicalRecord.ToString() + "\n");
    }
    
}