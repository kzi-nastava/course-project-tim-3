using MongoDB.Driver;
using MongoDB.Bson;

namespace Hospital
{
    public class MedicineRepository
    {
        private MongoClient _dbClient;
        public MedicineRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Checkup> GetAll()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Checkup>("medicines");
        }

        public void AddOrUpdateCheckup(Checkup newCheckup)
        {
            var checkups = GetAll();
            checkups.ReplaceOne(checkup => checkup.Id == newCheckup.Id, newCheckup, new ReplaceOptions {IsUpsert = true});
        }
    }
}