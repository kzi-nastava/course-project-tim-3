using MongoDB.Bson;
using MongoDB.Driver;
namespace Hospital;

public class DoctorUI : ConsoleUI
{
    public DoctorUI(Hospital _hospital, User _user) : base(_hospital) {}

     public override void Start()
    {
        bool quit = false;
        while (!quit)
        {
            Console.WriteLine("\nChoose an option below:\n\n1. View appointments for a specific day\n2. View timetable\n3. Create checkup\n4. Quit");
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
                    CreateCheckup();
                    break;
                }
                case "4":
                {
                    quit = true;
                    break;
                }
            }
        }
    }

    public bool CreateCheckup()
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
        if (patient == null)
        {
            Console.WriteLine("No such patient existst.");
            return false;
        }
        Doctor doctor = _hospital.DoctorRepo.GetById((ObjectId)_user.Person.Id);
        if (_hospital.AppointmentRepo.IsDoctorAvailable(dateTime, doctor))
        {
            Checkup checkup = new Checkup(dateTime, new MongoDBRef("patients", patient.Id), new MongoDBRef("doctors", _user.Person.Id), "anamnesis:");
            _hospital.AppointmentRepo.AddOrUpdateCheckup(checkup);
            Console.WriteLine("\nCheckup successfully added");
            return true;
        }
        else
        {
            Console.WriteLine("Doctor is not available at that time");
            return false;
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

        TimetableMenu(checkups);
    }

    public void TimetableMenu(List<Checkup> checkups)
    {
        bool quit = false;
        while (!quit)
        {
            Console.Write("\nOptions:\n\n1. See patient info for checkup\n2. Start checkup\n3. Update checkup\n4. Delete checkup\n5. Back\n");
            Console.Write(">>");
            var input = Console.ReadLine().Trim();
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
            _hospital.AppointmentRepo.DeleteCheckup(checkups[checkupNumber-1]);
            Console.WriteLine("Deletion successfull"); 
        }
        else
        {
            Console.WriteLine("\nWrong input. Please enter one of the given checkups.");
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
                    String anamnesis = Console.ReadLine();

                    patient.MedicalRecord.AnamnesisHistory.Add(anamnesis);
                    _hospital.PatientRepo.AddOrUpdatePatient(patient);

                    checkup.Anamnesis = anamnesis;
                    _hospital.AppointmentRepo.AddOrUpdateCheckup(checkup);

                    Console.Write("\nDo you want to add a prescription? [y/n] >> ");
                    String? choice = Console.ReadLine().ToLower();
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
                Console.Write("\nEnter new allergy >>");
                string allergy = Console.ReadLine();
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
            checkup.StartTime = newStartDate;
            _hospital.AppointmentRepo.AddOrUpdateCheckup(checkup);
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
        Patient newPatient = _hospital.PatientRepo.GetPatientByFullName(newName,newSurname);
        if (newPatient != null)
        {
           checkup.Patient = new MongoDB.Driver.MongoDBRef("patients", newPatient.Id);
            _hospital.AppointmentRepo.AddOrUpdateCheckup(checkup);                
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
        Doctor doctor = null;
        Console.Write("\nChoose specialty:\n1. Dermatology\n2. Radiology\n3. Stomatology\n4. Ophthalmology\n5. Family medicine>> ");
        string? specialty = Console.ReadLine();
        switch (specialty)
        {
            case "1":
            {
                doctor = _hospital.DoctorRepo.GetOneBySpecialty(Specialty.DERMATOLOGY);
                break;
            }
            case "2":
            {
                doctor = _hospital.DoctorRepo.GetOneBySpecialty(Specialty.RADIOLOGY);
                break;
            }
            case "3":
            {
                doctor = _hospital.DoctorRepo.GetOneBySpecialty(Specialty.STOMATOLOGY);
                break;
            }
            case "4":
            {
                doctor = _hospital.DoctorRepo.GetOneBySpecialty(Specialty.OPHTHALMOLOGY);
                break;
            }
            case "5":
            {
                doctor = _hospital.DoctorRepo.GetOneBySpecialty(Specialty.FAMILY_MEDICINE);
                break;
            }
            default:
            {
                Console.WriteLine("Wrong input");
                break;
            }
        }
        Console.WriteLine(doctor);
        if (doctor != null)
        {
            Referral referral = new Referral(new MongoDBRef("patients", patient.Id), new MongoDBRef("doctors", doctor.Id));
            patient.MedicalRecord.Referrals.Add(referral);
            _hospital.PatientRepo.AddOrUpdatePatient(patient);
            Console.WriteLine("\nReferral succesfully added");
        }
        else
        {
            Console.WriteLine("No adequate doctor found.");
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
            Doctor doctor = _hospital.DoctorRepo.GetByFullName(firstName, lastName);
            if (doctor != null)
            {
                Referral referral = new Referral(new MongoDBRef("patients", patient.Id), new MongoDBRef("doctors", doctor.Id));
                patient.MedicalRecord.Referrals.Add(referral);
                _hospital.PatientRepo.AddOrUpdatePatient(patient);
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
        bool quit = false;
        while (true)
        {
            Console.Write("\nEnter medicine name >> ");
            string? name = Console.ReadLine();
            Medicine medicine = _hospital.MedicineRepo.GetByName(name);

            if (medicine == null)
            {
                Console.WriteLine("No such medicine found in database");
                break;
            }
            if (patient.IsAllergicToMedicine(medicine)) 
            {
                Console.WriteLine("Patient is allergic to given medicine. Cancelling prescription.");
                break;
            }

            Console.Write("\nEnter amount of times the medicine should be taken a day >> ");
            int amount = Int32.Parse(Console.ReadLine());
            Console.Write("\nEnter amount of hours inbetween medicine intake >> ");
            int hours = Int32.Parse(Console.ReadLine());
            Console.Write("\nWhen to take in medicine:\n1. Before Meal\n2. After Meal\n3. With Meal\n4. Anytime\n>> ");
            string? bestTaken = Console.ReadLine();

            WritePrescription(medicine, amount, bestTaken, hours, patient);
            
            string? choice = "n";
            while (choice != "y")
            {
                Console.Write("\nDo you want to prescribe another medicine [y/n] >>");
                choice = Console.ReadLine();
                if (choice == "n")
                {
                    break;
                }
            }
            if (choice == "n") break;
        }
    }

    public void WritePrescription(Medicine medicine, int amount, string bestTaken, int hours, Patient patient)
    {
        switch (bestTaken)
        {
            case "1":
            {
                AddPrescription(medicine, amount, MedicineBestTaken.BEFORE_MEAL, hours, patient);
                break;
            }
            case "2":
            {
                AddPrescription(medicine, amount, MedicineBestTaken.AFTER_MEAL, hours, patient);
                break;
            }
            case "3":
            {
                AddPrescription(medicine, amount, MedicineBestTaken.WITH_MEAL, hours, patient);
                break;
            }
            case "4":
            {
                AddPrescription(medicine, amount, MedicineBestTaken.ANY_TIME, hours, patient);
                break;
            }
        }         
    }

    public void AddPrescription(Medicine medicine, int amount, MedicineBestTaken bestTaken, int hours, Patient patient)
    {
        Prescription prescription = new Prescription(medicine, amount, MedicineBestTaken.ANY_TIME, hours);
        patient.MedicalRecord.Prescriptions.Add(prescription);
        _hospital.PatientRepo.AddOrUpdatePatient(patient);
    }
}