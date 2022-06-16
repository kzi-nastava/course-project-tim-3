using HospitalSystem.Core;
using HospitalSystem.Core.Medications;
using HospitalSystem.Core.Medications.Requests;

namespace HospitalSystem.Tests;
public static class MedicationGenerator
{
    public static void GenerateMedication(Hospital hospital)
    {
        hospital.MedicationRepo.Upsert(new Medication("ibuprofen", new List<string> {"lactose", "Maize Starch", "Hypromellose", "sodium starch glycollate", "colloidal Anhydrous Silica", "magnesium Stearate", "sucrose", "talc", "titanium Dioxide (E171)", "carnauba Wax"}));
        hospital.MedicationRepo.Upsert(new Medication("probiotic", new List<string> {"lactobacillus"}));
        hospital.MedicationRepo.Upsert(new Medication("amoxicillin", new List<string> {"penicillin","magnesium Stearate (E572)", "Colloidal Anhydrous Silica"}));
        hospital.MedicationRepo.Upsert(new Medication("oxacillin", new List<string> {"penicillin"}));
    }

    public static void GenerateMedicationRequests(Hospital hospital)
    {
        hospital.MedicationRequestService.Send(new MedicationRequest(
            new Medication("ultra probiotic", new List<string> {"ultra lactobacillus"}), "ULTRA"));
        hospital.MedicationRequestService.Send(new MedicationRequest(
            new Medication("ultra amoxicillin", new List<string> {"ultra penicillin","mega magnesium Stearate (E572)", "Colloidal Anhydrous Silica"}), "ULTRA2"));
        hospital.MedicationRequestService.Send(new MedicationRequest(
            new Medication("ultra oxacillin", new List<string> {"ultra penicillin"}), "ULTRA3"));
    }
}