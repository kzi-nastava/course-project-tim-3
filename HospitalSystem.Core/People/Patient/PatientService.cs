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

    public IQueryable<Patient> GetPatients()
    {
        return _repo.GetPatients();
    }
    public void AddOrUpdatePatient(Patient patient)
    {
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
        return _repo.GetPatientByFullName(firstName,lastName);
    }
    public Patient GetPatientById(ObjectId id)
    {
        return _repo.GetPatientById(id);
    }

    public List<DateTime> GetAllTimesForMedicine (Prescription prescription)
    {
        DateTime now = DateTime.Now;
        //TODO: unhardcode this
        DateTime beginsToTake = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
        List<DateTime> times = new List<DateTime>();
        for (int i=0; i<prescription.TimesADay; i++)
        {
            DateTime timeToTake = beginsToTake.AddHours(i*prescription.HoursBetweenIntakes);
            times.Add(timeToTake);
        }

        return times;
    }

    //method that may return datetime for medicine that is in notification time window
    public DateTime ?WhenToTakeMedicine(Prescription prescription, Patient patient)
    {
        List<DateTime> timesForMedicine = GetAllTimesForMedicine(prescription);
        foreach (DateTime time in timesForMedicine)
        {
            if (DateTime.Now < time && DateTime.Now.Add(patient.WhenToRemind)>= time)
            {
                return time;
            }
        }
        return null;
    }
}