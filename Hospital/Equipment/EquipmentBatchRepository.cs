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

    public IMongoQueryable<EquipmentBatch> GetEquipmentBatchesInRoom(Room room)
    {
        var equipmentBatches = GetQueryableEquipmentBatches();
        var matches = 
            from equipmentBatch in equipmentBatches
            where equipmentBatch.Room.Id == room.Id
            select equipmentBatch;
        return matches;
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

    // NOTE: only use during Relocation!!
    public void RemoveEquipmentBatch(EquipmentBatch removingBatch)
    {
        var existingBatch = GetEquipmentBatch((ObjectId) removingBatch.Room.Id, removingBatch.Name);
        if (existingBatch is null)
        {
            throw new Exception("HOW DID YOU GET HERE?");  // TODO: change exception
        }
        else
        {
            existingBatch.Remove(removingBatch);
            UpdateEquipmentBatch(existingBatch);
        }
    }

    public void DeleteEquipmentBatchesInRoom(Room room)
    {
        GetEquipmentBatches().DeleteMany(batch => batch.Room.Id == room.Id);
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