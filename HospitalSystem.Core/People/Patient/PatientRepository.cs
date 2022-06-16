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

        public IMongoCollection<Patient> GetAll()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Patient>("patients");
        }
        public void Upsert(Patient patient)
        {
            var newPatient = patient;
            var patients = GetAll();
            patients.ReplaceOne(patient => patient.Id == newPatient.Id, newPatient, new ReplaceOptions {IsUpsert = true});
        }
        public Patient GetByFullName(string firstName, string lastName)
        {
            var patients = GetAll();
            var foundPatient = patients.Find(patient => patient.FirstName == firstName && patient.LastName == lastName)
                .FirstOrDefault();
            return foundPatient;
        }

        public Patient GetById(ObjectId id)
        {
            var patients = GetAll();
            var foundPatient = patients.Find(patient => patient.Id == id).FirstOrDefault();
            return foundPatient;
        }
    }
} 
