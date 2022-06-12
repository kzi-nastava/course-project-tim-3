using MongoDB.Bson;
using MongoDB.Driver;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

public class StartCheckupUI : DoctorCheckupsUI
{
    Checkup checkup;
    public StartCheckupUI(Hospital hospital, User user, Checkup checkup) : base(hospital, user) { }
    public override void Start()
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
                    addAnamnesis(patient);
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

    public void addAnamnesis(Patient patient)
    {
        Console.Write("\nEnter Anamnesis >> ");
        String? anamnesis = Console.ReadLine();

        patient.MedicalRecord.AnamnesisHistory.Add(anamnesis);
        _hospital.PatientService.AddOrUpdatePatient(patient);

        checkup.Anamnesis = anamnesis;
        _hospital.AppointmentService.UpsertCheckup(checkup);

        Console.Write("\nDo you want to add a prescription? [y/n] >> ");
        string choice = ReadSanitizedLine();
        if (choice == "y")
        {
            PrescriptionMenu(patient);
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
            _hospital.PatientService.AddOrUpdatePatient(patient);
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
            _hospital.PatientService.AddOrUpdatePatient(patient);
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
        _hospital.PatientService.AddOrUpdatePatient(patient);
        Console.WriteLine("Edit successfull");
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
            else
            {
                if (patient.IsAllergicToMedication(medication)) 
                {
                    Console.WriteLine("Patient is allergic to given Medication. Cancelling prescription.");
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
                    _hospital.EquipmentService.Remove(new EquipmentBatch(equipment.RoomLocation, equipment.Name, amount, equipment.Type));
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