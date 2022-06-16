using MongoDB.Bson;

namespace HospitalSystem.Core;

public interface IPatientRepository
{
    public IQueryable<Patient> GetPatients();

    public void AddOrUpdatePatient(Patient patient);

    public Patient GetPatientByFullName(string firstName, string lastName);
       
    public Patient GetPatientById(ObjectId id);
}
