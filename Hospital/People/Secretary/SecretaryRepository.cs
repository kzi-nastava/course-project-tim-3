using MongoDB.Driver;

namespace Hospital
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

    }
} 
