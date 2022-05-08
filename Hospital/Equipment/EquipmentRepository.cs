using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace Hospital
{
    public class EquipmentRepository
    {
        private MongoClient _dbClient;

        public EquipmentRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Equipment> GetEquipments()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Equipment>("equipments");
        }

        public IMongoQueryable<Equipment> GetQueryableEquipments()
        {
            return GetEquipments().AsQueryable();
        }

        public void AddEquipment(Equipment newEquipment)
        {
            var equipments = GetEquipments();
            var existingEquipment = equipments.Find(equipment => equipment.Room.Id == newEquipment.Room.Id 
                                                                 && equipment.Name == newEquipment.Name);
            if (!existingEquipment.Any())
            {
                equipments.InsertOne(newEquipment);
            }
            else
            {
                var equipment = existingEquipment.First();
                equipment.MergeWith(newEquipment);
                UpdateEquipment(equipment);
            }
        }

        public void UpdateEquipment(Equipment newEquipment) // EXPECTS EXISTING EQUIPMENT!
        {
            var equipments = GetEquipments();
            equipments.ReplaceOne(equipment => equipment.Id == newEquipment.Id, newEquipment);
        }
    }
}