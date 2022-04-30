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

    }
} 
