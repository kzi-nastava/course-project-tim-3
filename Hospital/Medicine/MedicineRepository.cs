using MongoDB.Driver;
using MongoDB.Bson;

namespace Hospital
{
    public class MedicationRepository
    {
        private MongoClient _dbClient;
        public MedicationRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Medication> GetAll()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Medication>("medications");
        }

        public void AddOrUpdate(Medication newMedication)
        {
            var medications = GetAll();
            medications.ReplaceOne(medication => medication.Id == newMedication.Id, newMedication, new ReplaceOptions {IsUpsert = true});
        }

        public Medication GetByName(string name)
        {
            var medications = GetAll();
            return medications.Find(medication => medication.Name == name).FirstOrDefault();
        }
    }
}