using MongoDB.Bson;
using MongoDB.Driver;
namespace Hospital;

public class DoctorUI : ConsoleUI
{
    public DoctorUI(Hospital _hospital, User _user) : base(_hospital) {}

     public override void Start()
    {
        bool exit = false;
        while (!exit)
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
                    exit = true;
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
        bool exit = false;
        while (!exit)
        {
            Console.Write("\nOptions:\n\n1. See patient info for checkup\n2. Start checkup\n3. Update checkup\n4. Delete checkup\n5. Add checkup\n6. Back\n");
            Console.Write(">>");
            var input = Console.ReadLine().Trim();
            switch (input)
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
                    Console.Write("\nEnter checkup number >> ");
                    try
                    {
                        var checkupNumber = int.Parse(Console.ReadLine());
                        StartCheckup(checkups[checkupNumber]);
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
                case "3":
                {
                    Console.Write("\nEnter checkup number >> ");
                    try
                    {
                        var checkupNumber = int.Parse(Console.ReadLine());
                        Checkup checkup = checkups[checkupNumber-1];
                        EditCheckup(checkup);
                    } catch (IOException e)
                    {
                        Console.WriteLine("Wrong input1.");
                    } catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("Wrong input2.");
                    } catch (NullReferenceException e)
                    {
                        Console.WriteLine("Wrong input3.");
                    }
                    break;
                }
                case "4":
                {
                    Console.Write("\nEnter checkup number >> ");
                    try
                    {
                        var checkupNumber = int.Parse(Console.ReadLine());
                        _hospital.AppointmentRepo.DeleteCheckup(checkups[checkupNumber-1]);
                        Console.WriteLine("Deletion successfull");
                    } catch (IOException e)
                    {
                        Console.WriteLine("Wrong input1.");
                    } catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("Wrong input2.");
                    } catch (NullReferenceException e)
                    {
                        Console.WriteLine("Wrong input3.");
                    }                    
                    break;
                }
                case "5":
                {
                    Console.WriteLine("Creating new Checkup appointment...");
                    Console.Write("\nEnter date >>");
                    string? date = Console.ReadLine();
                    Console.Write("\nEnter time >>");
                    string? time = Console.ReadLine();
                    DateTime dateTime = DateTime.Parse(date + " " + time);
                    Console.Write("\nEnter patient name >>");
                    string? name = Console.ReadLine();
                    Console.Write("\nEnter patient surname >>");
                    string? surname = Console.ReadLine();
                    Patient patient = _hospital.PatientRepo.GetPatientByFullName(name,surname);
                    Checkup checkup = new Checkup(dateTime, new MongoDBRef("patients", patient.Id), new MongoDBRef("doctors", _user.Person.Id), "anamnesis:");
                    _hospital.AppointmentRepo.AddOrUpdateCheckup(checkup);
                    break;
                }
                case "6":
                {
                    exit = true;
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
            Console.WriteLine(String.Format("{0,5} {1,12} {2,12} {3,25}", i, checkup.StartTime.ToString("dd.MM.yyyy"), 
            checkup.StartTime.ToString("HH:mm"), patient));
            i++;
        }
    }

    public Patient ShowPatientInfo(Checkup checkup)
    {
        Patient patient = _hospital.PatientRepo.GetPatientById((ObjectId)checkup.Patient.Id);
        Console.Write("\n" + patient.ToString() + "\n");
        Console.Write(patient.MedicalRecord.ToString() + "\n");
        return patient;
    }

    public void StartCheckup(Checkup checkup)
    {
        Patient patient =  ShowPatientInfo(checkup);
        Console.WriteLine("\n\nCheckup started.\n");
        Console.Write("\nCheckup options:\n\n1. Add Anamnesis\n2. Edit Medical Record\n3. Back\n\n");
        Console.Write(">>");
        var input = Console.ReadLine();
        switch (input)
        {
        case "1":
            {
                Console.Write("\nEnter Anamnesis >> ");
                String anamnesis = Console.ReadLine();
                patient.MedicalRecord.AnamnesisHistory.Add(anamnesis);
                _hospital.PatientRepo.AddOrUpdatePatient(patient);
                break;
            }
            case "2":
            {
                EditMedicalRecord(patient);
                break;
            }
            case "3":
            {
                break;
            }
        }
    }

    public void EditMedicalRecord(Patient patient)
    {
        Console.Write("\nEdit options:\n1. Edit height\n2. Edit weight\n3. Add allergy\n4. Back\n");
        Console.Write(">>");
        var input = Console.ReadLine();
        switch (input)
        {
            case "1":
            {
                Console.Write("\nEnter new height >>");
                var height = double.Parse(Console.ReadLine());
                patient.MedicalRecord.HeightInCm = height;
                _hospital.PatientRepo.AddOrUpdatePatient(patient);
                Console.WriteLine("Edit successfull");
                break;
            }
            case "2":
            {
                Console.Write("\nEnter new weight >>");
                var weight = double.Parse(Console.ReadLine());
                patient.MedicalRecord.WeightInKg = weight;
                _hospital.PatientRepo.AddOrUpdatePatient(patient);
                Console.WriteLine("Edit successfull");

                break;
            }
            case "3":
            {
                Console.Write("\nEnter new allergy >>");
                var allergy = Console.ReadLine();
                patient.MedicalRecord.Allergies.Add(allergy);
                _hospital.PatientRepo.AddOrUpdatePatient(patient);
                Console.WriteLine("Edit successfull");
                break;
            }
            case "4":
            {
                break;
            }
        }
    }

    public void EditCheckup(Checkup checkup)
    {
        Console.WriteLine("\n\nEdit checkup.\n");
        Console.Write("\nEdit options:\n\n1. Edit date and time\n2. Edit Patient\n3. Back\n\n");
        Console.Write(">>");
        var editInput = Console.ReadLine();
        switch (editInput) 
        {
            case "1":
            {
                Console.Write("Enter new date >> ");
                string? date = Console.ReadLine();
                Console.Write("Enter new time >> ");
                string? time = Console.ReadLine();
                DateTime newDateTime = DateTime.Parse(date + " " + time);
                Console.Write(newDateTime);
                checkup.StartTime = newDateTime;
                _hospital.AppointmentRepo.AddOrUpdateCheckup(checkup);
                Console.WriteLine("Edit successfull");
                break;
            }
            case "2":
            {
                Console.Write("Enter new patient name>> ");
                string? newName = Console.ReadLine();
                Console.Write("Enter new patient surname>> ");
                string? newSurname = Console.ReadLine();
                checkup.Patient = new MongoDB.Driver.MongoDBRef("patients", _hospital.PatientRepo.GetPatientByFullName(newName,newSurname).Id);
                _hospital.AppointmentRepo.AddOrUpdateCheckup(checkup);                
                Console.WriteLine("Edit successfull");
                break;
            }
        }
    }
}