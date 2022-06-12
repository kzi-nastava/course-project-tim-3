using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace HospitalSystem.Core;

public interface IPatientRepository
{

    public IMongoCollection<Patient> GetMongoCollection();

    public IQueryable<Patient> GetPatients();

    public void AddOrUpdatePatient(Patient patient);

    public Patient GetPatientByFullName(string firstName, string lastName);
       
    public Patient GetPatientById(ObjectId id);
}
