using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem.Core
{
    public class PatientRepository : IPatientRepository
    {
        private MongoClient _dbClient;

        public PatientRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Patient> GetMongoCollection()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Patient>("patients");
        }

        public IQueryable<Patient> GetAll()
        {
            return GetMongoCollection().AsQueryable();
        }
        public void Upsert(Patient patient)
        {
            var newPatient = patient;
            var patients = GetMongoCollection();
            patients.ReplaceOne(patient => patient.Id == newPatient.Id, newPatient, new ReplaceOptions {IsUpsert = true});
        }
        public Patient GetByFullName(string firstName, string lastName)
        {
            var patients = GetMongoCollection();
            var foundPatient = patients.Find(patient => patient.FirstName == firstName && patient.LastName == lastName)
                .FirstOrDefault();
            return foundPatient;
        }

        public Patient GetById(ObjectId id)
        {
            var patients = GetMongoCollection();
            var foundPatient = patients.Find(patient => patient.Id == id).FirstOrDefault();
            return foundPatient;
        }
    }
} 
