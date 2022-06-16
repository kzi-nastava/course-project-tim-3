using MongoDB.Bson;

namespace HospitalSystem.Core;

public interface IPatientRepository
{
    public IQueryable<Patient> GetAll();

    public void Upsert(Patient patient);

    public Patient GetByFullName(string firstName, string lastName);
       
    public Patient GetById(ObjectId id);
}
