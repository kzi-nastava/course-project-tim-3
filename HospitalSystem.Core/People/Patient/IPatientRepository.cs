using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace HospitalSystem.Core;

public interface IPatientRepository
{
    public IMongoCollection<Patient> GetAll();

    public void Upsert(Patient patient);

    public Patient GetByFullName(string firstName, string lastName);
       
    public Patient GetById(ObjectId id);
}
