using MongoDB.Bson;
using MongoDB.Driver;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

public class DoctorUI : UserUI
{
    public DoctorUI(Hospital hospital, User user) : base(hospital, user) { }

     public override void Start()
    {
        bool quit = false;
        while (!quit)
        {
            Console.WriteLine("\nChoose an option below:\n\n1. View appointments for a specific day\n2. View timetable\n3. Create checkup\n4. Manage medication requests\n5. Quit");
            Console.Write("\n>>");
            var option = ReadSanitizedLine().Trim();
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
                    CreateCheckup();
                    break;
                }
                case "4":
                {
                    MedicationRequestsMenu();
                    break;
                }
                case "5":
                {
                    quit = true;
                    break;
                }
            }
        }
    }

    public void CreateCheckup()
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
        if (_hospital.AppointmentService.UpsertCheckup(_user, dateTime, name, surname) == true)
        {
            Console.WriteLine("\nCheckup successfully added");
        }
        else
        {
            Console.WriteLine("Doctor is not available at that time");
        }
    }

    public void ShowCheckupsByDay()
    {
        Console.Write("\nEnter date (dd.mm.yyyy) >> ");
        var date = Console.ReadLine();
        List<Checkup> checkups = _hospital.AppointmentService.GetCheckupsByDay(Convert.ToDateTime(date));
        PrintCheckups(checkups);
    }

    public void ShowNextThreeDays()
    {
        Console.WriteLine("\nThese are your checkups for the next 3 days:\n");
        List<Checkup> checkups = _hospital.AppointmentService.GetCheckupsByDay(DateTime.Now);
        checkups.AddRange(_hospital.AppointmentService.GetCheckupsByDay(DateTime.Today.AddDays(1)));
        checkups.AddRange(_hospital.AppointmentService.GetCheckupsByDay(DateTime.Today.AddDays(2)));
        PrintCheckups(checkups);
        TimetableMenu(checkups);
    }

    public void TimetableMenu(List<Checkup> checkups)
    {
        bool quit = false;
        while (!quit)
        {
            Console.Write("\nOptions:\n\n1. See patient info for checkup\n2. Start checkup\n3. Update checkup\n4. Delete checkup\n5. Back\n");
            Console.Write(">>");
            var input = ReadSanitizedLine().Trim();
            switch (input)
            {
                case "1":
                {
                    ShowInfoMenu(checkups);
                    break;
                }
                case "2":
                {
                    StartCheckupMenu(checkups);
                    break;
                }
                case "3":
                {
                    EditCheckupMenu(checkups);
                    break;
                }
                case "4":
                {
                    DeleteCheckupMenu(checkups);
                    break;
                }
                case "5":
                {
                    quit = true;
                    break;
                }
            }
        }
    }

    public void ShowInfoMenu(List<Checkup> checkups)
    {
        Console.Write("\nEnter checkup number >> ");
        var isNumber = int.TryParse(Console.ReadLine(), out int checkupNumber);
        if (isNumber == true && checkupNumber >= 0 && checkupNumber <= checkups.Count())
        {
            ShowPatientInfo(checkups[checkupNumber-1]);
        }
        else
        {
            Console.WriteLine("\nWrong input. Please enter one of the given checkups.");
        }
    }

    public void StartCheckupMenu(List<Checkup> checkups)
    {
        Console.Write("\nEnter checkup number >> ");
        var isNumber = int.TryParse(Console.ReadLine(), out int checkupNumber);
        if (isNumber == true && checkupNumber >= 0 && checkupNumber <= checkups.Count())
        {
            StartCheckup(checkups[checkupNumber-1]);
        }
        else
        {
            Console.WriteLine("\nWrong input. Please enter one of the given checkups.");
        }
    }

    public void EditCheckupMenu(List<Checkup> checkups)
    {
        Console.Write("\nEnter checkup number >> ");
        var isNumber = int.TryParse(Console.ReadLine(), out int checkupNumber);
        if (isNumber == true && checkupNumber >= 0 && checkupNumber <= checkups.Count())
        {
            Checkup checkup = checkups[checkupNumber-1];
            EditCheckup(checkup);
        }
        else
        {
            Console.WriteLine("\nWrong input. Please enter one of the given checkups.");
        }
    }

    public void DeleteCheckupMenu(List<Checkup> checkups)
    {
        Console.Write("\nEnter checkup number >> ");
        var isNumber = int.TryParse(Console.ReadLine(), out int checkupNumber);
        if (isNumber == true && checkupNumber >= 0 && checkupNumber <= checkups.Count())
        {
            _hospital.AppointmentService.DeleteCheckup(checkups[checkupNumber-1]);
            Console.WriteLine("Deletion successfull"); 
        }
        else
        {
            Console.WriteLine("\nWrong input. Please enter one of the given checkups.");
        }
    }

    public void PrintCheckups(List<Checkup> checkups)
    {
        Console.WriteLine(String.Format("{0,5} {1,24} {2,25}", "Nr.", "Date & Time", "Patient"));
        int i = 1;
        foreach (Checkup checkup in checkups)
        {
            Patient patient = _hospital.PatientRepo .GetPatientById((ObjectId)checkup.Patient.Id);
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 60)));
            Console.WriteLine(String.Format("{0,5} {1,24} {2,25}", i, checkup.DateRange, patient));
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
        bool quit = false;
        Patient patient =  ShowPatientInfo(checkup);
        Console.WriteLine("\n\nCheckup started.\n");
        while (!quit)
        {
            Console.Write("\nCheckup options:\n\n1. Add Anamnesis\n2. Edit Medical Record\n3. Write referral\n4. Back\n\n>>");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                {
                    Console.Write("\nEnter Anamnesis >> ");
                    String? anamnesis = Console.ReadLine();

                    patient.MedicalRecord.AnamnesisHistory.Add(anamnesis);
                    _hospital.PatientRepo.AddOrUpdatePatient(patient);

                    checkup.Anamnesis = anamnesis;
                    _hospital.AppointmentService.UpsertCheckup(checkup);

                    Console.Write("\nDo you want to add a prescription? [y/n] >> ");
                    string choice = ReadSanitizedLine();
                    if (choice == "y")
                    {
                        PrescriptionMenu(patient);
                    }
                    break;
                }
                case "2":
                {
                    EditMedicalRecord(patient);
                    break;
                }
                case "3":
                {
                    WriteReferral(patient);
                    break;
                }
                case "4":
                {
                    EquipmentStateUpdate(checkup);
                    quit = true;
                    break;
                }
                default:
                {
                    Console.WriteLine("Wrong input. Please enter one of the options above.");
                    break;
                }
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
                EditHeight(patient);
                break;
            }
            case "2":
            {
                EditWeight(patient);
                break;
            }
            case "3":
            {
                EditAllergies(patient);
                break;
            }
            case "4":
            {
                break;
            }
        }
    }

    public void EditWeight(Patient patient)
    {
        Console.Write("\nEnter new weight in kg >>");
        var input = int.TryParse(Console.ReadLine(), out int weight);
        if (input == true && weight > 10 && weight < 400)
        {
            patient.MedicalRecord.WeightInKg = weight;
            _hospital.PatientRepo.AddOrUpdatePatient(patient);
            Console.WriteLine("Edit successfull");
        }
        else
        {
            Console.WriteLine("Please enter a valid number");
        }
    }

    public void EditHeight(Patient patient)
    {
        Console.Write("\nEnter new height in cm >>");
        var input = int.TryParse(Console.ReadLine(), out int height);
        if (input == true && height > 30 && height < 250)
        {
            patient.MedicalRecord.HeightInCm = height;
            _hospital.PatientRepo.AddOrUpdatePatient(patient);
            Console.WriteLine("Edit successfull");
        }
        else
        {
            Console.WriteLine("Please enter a valid number");
        }
    }

    public void EditAllergies(Patient patient)
    {
        Console.Write("\nEnter new allergy >>");
        string? allergy = Console.ReadLine();
        patient.MedicalRecord.Allergies.Add(allergy);
        _hospital.PatientRepo.AddOrUpdatePatient(patient);
        Console.WriteLine("Edit successfull");
    }

    public void EditCheckup(Checkup checkup)
    {
        Console.WriteLine("\n\nEdit checkup.\n");
        Console.Write("\nEdit options:\n\n1. Edit start date\n2. Edit Patient\n3. Back\n\n");
        Console.Write(">>");
        var editInput = Console.ReadLine();
        switch (editInput) 
        {
            case "1":
            {
                EditStartTime(checkup);
                break;
            }
            case "2":
            {
                EditCheckupPatient(checkup);
                break;
            }
        }
    }

    public void EditStartTime(Checkup checkup)
    {
        Console.Write("Enter new date >> ");
        string? date = Console.ReadLine();
        Console.Write("Enter new time >> ");
        string? time = Console.ReadLine();
        var newDateTime = DateTime.TryParse(date + " " + time, out DateTime newStartDate);
        if (newDateTime == true)
        {
            checkup.DateRange = new DateRange(newStartDate, newStartDate.Add(Checkup.DefaultDuration), allowPast: false);
            _hospital.AppointmentService.UpsertCheckup(checkup);
            Console.WriteLine("\nEdit successfull");
        }
        else
        {
            Console.WriteLine("\nPlease enter valid date and time");
        }
    }

    public void EditCheckupPatient(Checkup checkup)
    {
        Console.Write("Enter new patient name>> ");
        string? newName = Console.ReadLine();
        Console.Write("Enter new patient surname>> ");
        string? newSurname = Console.ReadLine();
        Patient newPatient = _hospital.PatientService.GetPatientByFullName(newName,newSurname);
        if (newPatient != null)
        {
           checkup.Patient = new MongoDB.Driver.MongoDBRef("patients", newPatient.Id);
            _hospital.AppointmentService.UpsertCheckup(checkup);                
            Console.WriteLine("Edit successfull"); 
        }
        else
        {
            Console.WriteLine("\nNo such patient found.");
        }
    }

    public void WriteReferral(Patient patient)
    {
        Console.Write("\nRefferal by specialty or doctor [s/d] >> ");
        string? option = Console.ReadLine();
        switch (option)
        {
            case "s":
            {
                ReferralBySpecialtyMenu(patient);
                break;
            }
            case "d":
            {
                ReferralByDoctor(patient);
                break;
            }
        }
    }

    public void ReferralBySpecialtyMenu(Patient patient)
    {
        Console.Write("\nChoose specialty:\n1. Dermatology\n2. Radiology\n3. Stomatology\n4. Ophthalmology\n5. Family medicine>> ");
        var input = Int32.TryParse(Console.ReadLine(), out int specialty);
        if (input)
        {
            Doctor doctor = _hospital.DoctorService.GetOneBySpecialty((Specialty)specialty);
            if (doctor != null)
            {
                _hospital.PatientService.AddReferral(patient, doctor);
            }
            else
            {
                Console.WriteLine("No adequate doctor found.");
            }
        }
        else
        {
            Console.Write("\nPlease input valid option.");
        }
        
    }

    public void ReferralByDoctor(Patient patient)
    {
        Console.Write("\nEnter doctor's first name >> ");
        string? firstName = Console.ReadLine();
        Console.Write("\nEnter doctor's last name >> ");
        string? lastName = Console.ReadLine();
        if (firstName != null && lastName != null)
        {
            Doctor doctor = _hospital.DoctorService.GetByFullName(firstName, lastName);
            if (doctor != null)
            {
                _hospital.PatientService.AddReferral(patient,doctor);
                Console.WriteLine("\nReferral succesfully added");
            }
            else
            {
                Console.WriteLine("No such doctor exists");
            } 
        }
        else 
        {
            Console.WriteLine("Wrong input");
        }   
    }

    public void PrescriptionMenu(Patient patient)
    {
        while (true)
        {
            Console.Write("\nEnter medication name >> ");
            string name = ReadSanitizedLine();
            Medication medication = _hospital.MedicationRepo.GetByName(name);

            if (medication == null)
            {
                Console.WriteLine("No such medication found in database");
            }
            else if (patient.IsAllergicToMedication(medication)) 
            {
                Console.WriteLine("Patient is allergic to given Medication. Cancelling prescription.");
                break;
            }
            else
            {
                Console.Write("\nEnter amount of times the medication should be taken a day >> ");
                int amount = Int32.Parse(ReadSanitizedLine());
                Console.Write("\nEnter amount of hours inbetween medication intake >> ");
                int hours = Int32.Parse(ReadSanitizedLine());
                Console.Write("\nWhen to take in medication:\n1. Before Meal\n2. After Meal\n3. With Meal\n4. Anytime\n>> ");
                var input = Int32.TryParse(ReadSanitizedLine(), out int bestTaken);
                if (input)
                    _hospital.PatientService.AddPrescription(medication, amount, (MedicationBestTaken)bestTaken, hours, patient);
                else
                    Console.Write("\nPlease enter a valid option.");
            }

            string? choice = "n";
            while (choice != "y")
            {
                Console.Write("\nDo you want to prescribe another medication [y/n] >>");
                choice = Console.ReadLine();
                if (choice == "n")
                {
                    break;
                }
            }
            if (choice == "n") break;
        }
    }

    public void MedicationRequestsMenu()
    {
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
                MedicationRequest request = requested[requestNumber-1];
                Console.Write("\n" + request);
                Console.WriteLine("\n1. Approve\n2. Deny\n3. Back");
                Console.WriteLine(">>");
                string? option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                    {
                        _hospital.MedicationRequestService.Approve(request);
                        Console.WriteLine("Request approved.");
                        back = true;
                        break;
                    }
                    case "2":
                    {
                        Console.Write("\nWrite comment >>");
                        string? comment = Console.ReadLine();
                        if (comment != null)
                            request.DoctorComment = comment;
                        _hospital.MedicationRequestService.Deny(request);
                        Console.WriteLine("\nRequest denied.");
                        back = true;
                        break;
                    }
                    case "3":
                    {
                        back = true;
                        break;
                    }
                    default:
                    {
                        Console.WriteLine("Wrong input. Please enter a valid option.");
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Wrong input. Please enter a valid option.");
            }
        }
    }

    public void EquipmentStateUpdate(Checkup checkup)
    {
        List<EquipmentBatch> equipments = _hospital.EquipmentService.GetAllIn(checkup.RoomLocation).ToList();
        PrintEquipmentState(equipments);

        foreach (EquipmentBatch equipment in equipments)
        {
            while (true)
            {
               Console.Write("\nInsert amount of used " + equipment.Name + " >> ");
                var isNumber = int.TryParse(Console.ReadLine(), out int amount);
                if (isNumber == true && amount >= 0 && amount <=equipment.Count)
                {
                    _hospital.EquipmentService.RemoveSome(equipment, amount);
                    break;
                }
                else
                {
                    Console.WriteLine("\nPlease enter a valid amount between 0 and " + equipment.Count);
                } 
            }
        }
    }

    public void PrintEquipmentState(List<EquipmentBatch> equipments)
    {
        Console.Write("\nEquipment state before checkup:\n\n");
        foreach (EquipmentBatch equipment in equipments)
        {
            Console.WriteLine(equipment);
        }   
    }
}