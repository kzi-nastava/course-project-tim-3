using MongoDB.Driver;

namespace Hospital
{
    public class DirectorRepository
    {
        private MongoClient _dbClient;

        public DirectorRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Director> GetDirectors()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Director>("directors");
        }

    }
} 
