using MongoDB.Driver;

namespace HospitalSystem.Core
{
    public class SecretaryRepository
    {
        private MongoClient _dbClient;

        public SecretaryRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Secretary> GetSecretaries()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Secretary>("secretaries");
        }

         public void AddOrUpdateSecretary(Secretary secretary)
        {
            var newSecretary = secretary;
            var secretaries = GetSecretaries();
            secretaries.ReplaceOne(secretary => secretary.Id == newSecretary.Id, newSecretary, new ReplaceOptions {IsUpsert = true});
        }
    }
} 
