using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem
{
    public class MedicineRepository
    {
        private MongoClient _dbClient;
        public MedicineRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Medicine> GetAll()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Medicine>("medicines");
        }

        public void AddOrUpdate(Medicine newMedicine)
        {
            var medicines = GetAll();
            medicines.ReplaceOne(medicine => medicine.Id == newMedicine.Id, newMedicine, new ReplaceOptions {IsUpsert = true});
        }

        public Medicine GetByName(string name)
        {
            var medicines = GetAll();
            return medicines.Find(medicine => medicine.Name == name).FirstOrDefault();
        }
    }
}