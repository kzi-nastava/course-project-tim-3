using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;

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

    public IMongoCollection<Patient> GetPatients()
    {
        return _repo.GetPatients();
    }
    public void AddOrUpdatePatient(Patient patient)
    {
        _repo.AddOrUpdatePatient(patient);
    }
    public Patient GetPatientByName(string name)
    {
        return _repo.GetPatientByName(name);
    }

    public Patient GetPatientByFullName(string firstName, string lastName)
    {
        return _repo.GetPatientByFullName(firstName,lastName);
    }
    public Patient GetPatientById(ObjectId id)
    {
        return _repo.GetPatientById(id);
    }
    
}