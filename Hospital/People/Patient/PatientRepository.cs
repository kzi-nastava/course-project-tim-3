using MongoDB.Driver;

namespace Hospital
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
        public void AddPatient(Patient patient)
        {
            var newPatient = patient;
            var patients = GetPatients();
            patients.ReplaceOne(patient => patient.Id == newPatient.Id, newPatient, new ReplaceOptions {IsUpsert = true});
        }
        public Patient getPatientByName(string name)
        {
            var patients = GetPatients();
            var foundPatient = patients.Find(patient => patient.FirstName == name).FirstOrDefault();
            return foundPatient;
        }
    }
} 
