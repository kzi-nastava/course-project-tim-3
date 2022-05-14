using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem
{
    public class PatientRepository
    {
        private MongoClient _dbClient;

        public PatientRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Patient> GetPatients()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Patient>("patients");
        }
        public void AddOrUpdatePatient(Patient patient)
        {
            var newPatient = patient;
            var patients = GetPatients();
            patients.ReplaceOne(patient => patient.Id == newPatient.Id, newPatient, new ReplaceOptions {IsUpsert = true});
        }
        public Patient GetPatientByName(string name)
        {
            var patients = GetPatients();
            var foundPatient = patients.Find(patient => patient.FirstName == name).FirstOrDefault();
            return foundPatient;
        }

        public Patient GetPatientByFullName(string firstName, string lastName)
        {
            var patients = GetPatients();
            var foundPatient = patients.Find(patient => patient.FirstName == firstName && patient.LastName == lastName).FirstOrDefault();
            return foundPatient;
        }
        public Patient GetPatientById(ObjectId id)
        {
            var patients = GetPatients();
            var foundPatient = patients.Find(patient => patient.Id == id).FirstOrDefault();
            return foundPatient;
        }
    }
} 
