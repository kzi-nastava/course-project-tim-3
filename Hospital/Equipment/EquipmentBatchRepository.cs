using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace Hospital;

public class EquipmentBatchRepository
{
    private MongoClient _dbClient;

    public EquipmentBatchRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }

    public IMongoCollection<EquipmentBatch> GetEquipmentBatches()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<EquipmentBatch>("equipments");
    }

    public IMongoQueryable<EquipmentBatch> GetQueryableEquipmentBatches()
    {
        return GetEquipmentBatches().AsQueryable();
    }

    public void AddEquipmentBatch(EquipmentBatch newEquipmentBatch)
    {
        var equipmentBatch = GetEquipmentBatch((ObjectId) newEquipmentBatch.Room.Id, newEquipmentBatch.Name);
        if (equipmentBatch is null)
        {
            GetEquipmentBatches().InsertOne(newEquipmentBatch);
        }
        else
        {
            equipmentBatch.MergeWith(newEquipmentBatch);
            UpdateEquipmentBatch(equipmentBatch);
        }
    }

    private void UpdateEquipmentBatch(EquipmentBatch newEquipmentBatch) // EXPECTS EXISTING EQUIPMENTBATCH!
    {
        var equipmentBatches = GetEquipmentBatches();
        equipmentBatches.ReplaceOne(equipmentBatch => equipmentBatch.Id == newEquipmentBatch.Id, newEquipmentBatch);
    }

    public EquipmentBatch? GetEquipmentBatch(ObjectId roomId, string name)
    {
        var equipmentBatches = GetEquipmentBatches();
        return equipmentBatches.Find(equipment => equipment.Room.Id == roomId && equipment.Name == name).FirstOrDefault();
    }
}