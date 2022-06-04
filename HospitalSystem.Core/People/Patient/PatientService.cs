using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem.Core;

public class PatientService
{
    private IPatientRepository _repo;

    public PatientService(IPatientRepository repo)
    {
        _repo = repo;
    }

    public bool WillNextCRUDOperationBlock(CRUDOperation crudOperation, Patient patient)
    {
        int limit;
        //TODO: unhardcode this
        switch (crudOperation)
        {
            case CRUDOperation.CREATE:
                limit = 8;
                break;
            case CRUDOperation.UPDATE:
                limit = 4;
                break;
            case CRUDOperation.DELETE:
                limit = 4;
                break;
            default:
                //this is dummy value, as of now there are no read restrictions
                limit = 999;
                break;
        }

        int count = 0;
        foreach (CheckupChangeLog log in patient.CheckupChangeLogs)
        {
            if (log.StartTime > DateTime.Now.AddDays(-30) &&  log.CRUDOperation == crudOperation)
            {
                count++;
            }
        }

        if (count+1 > limit)
        {
            return true;
        }
        return false;
    }

    public void LogChange(CRUDOperation crudOperation, Patient patient)
    {
        CheckupChangeLog log = new CheckupChangeLog(DateTime.Now,crudOperation);
        patient.CheckupChangeLogs.Add(log);
        _repo.AddOrUpdatePatient(patient);
    }

    public void AddPrescription(Medication medication, int amount, MedicationBestTaken bestTaken, int hours, Patient patient)
    {
        if (patient.IsAllergicToMedication(medication)) 
        {
            Console.WriteLine("Patient is allergic to given Medication. Cancelling prescription.");
        }
        else
        {
            Prescription prescription = new Prescription(medication, amount, bestTaken, hours);
            patient.MedicalRecord.Prescriptions.Add(prescription);
            _repo.AddOrUpdatePatient(patient);
        }  
    }

    public void AddReferral(Patient patient, Doctor doctor)
    {
        Referral referral = new Referral(new MongoDBRef("patients", patient.Id), new MongoDBRef("doctors", doctor.Id));
        patient.MedicalRecord.Referrals.Add(referral);
        _repo.AddOrUpdatePatient(patient);
        Console.WriteLine("\nReferral succesfully added");
    }

    public Patient GetPatientByFullName(string firstName, string lastName)
    {
        var patients = _repo.GetPatients();
        var foundPatient = patients.Find(patient => patient.FirstName == firstName && patient.LastName == lastName).FirstOrDefault();
        return foundPatient;
    }

    public void AddOrUpdatePatient(Patient patient)
    {
        _repo.AddOrUpdatePatient(patient);
    }

    public Patient GetPatientById(ObjectId id)
    {
        var patients = _repo.GetPatients();
        var foundPatient = patients.Find(patient => patient.Id == id).FirstOrDefault();
        return foundPatient;
    }
}