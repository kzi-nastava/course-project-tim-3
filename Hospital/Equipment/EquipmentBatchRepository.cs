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

    private IMongoCollection<EquipmentBatch> GetCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<EquipmentBatch>("equipments");
    }

    public IQueryable<EquipmentBatch> GetAll()
    {
        return GetCollection().AsQueryable();
    }

    public IQueryable<EquipmentBatch> GetAllInRoom(Room room)
    {
        var equipmentBatches = GetAll();
        var matches = 
            from equipmentBatch in equipmentBatches
            where equipmentBatch.Room.Id == room.Id
            select equipmentBatch;
        return matches;
    }

    public void Add(EquipmentBatch newEquipmentBatch)
    {
        var equipmentBatch = Get((ObjectId) newEquipmentBatch.Room.Id, newEquipmentBatch.Name);
        if (equipmentBatch is null)
        {
            GetCollection().InsertOne(newEquipmentBatch);
        }
        else
        {
            equipmentBatch.MergeWith(newEquipmentBatch);
            Replace(equipmentBatch);
        }
    }

    // NOTE: only use during Relocation!!
    public void Remove(EquipmentBatch removingBatch)
    {
        var existingBatch = Get((ObjectId) removingBatch.Room.Id, removingBatch.Name);
        if (existingBatch is null)
        {
            throw new Exception("HOW DID YOU GET HERE?");  // TODO: change exception
        }
        else
        {
            existingBatch.Remove(removingBatch);
            if (existingBatch.Count != 0)
                Replace(existingBatch);
            else
                GetCollection().DeleteOne(batch => batch.Id == existingBatch.Id);
        }
    }

    public void DeleteInRoom(Room room)
    {
        GetCollection().DeleteMany(batch => batch.Room.Id == room.Id);
    }

    private void Replace(EquipmentBatch newEquipmentBatch) // EXPECTS EXISTING EQUIPMENTBATCH!
    {
        GetCollection().ReplaceOne(equipmentBatch => equipmentBatch.Id == newEquipmentBatch.Id, newEquipmentBatch);
    }

    public EquipmentBatch? Get(ObjectId roomId, string name)
    {
        var equipmentBatches = GetCollection();
        return equipmentBatches.Find(equipment => equipment.Room.Id == roomId && equipment.Name == name).FirstOrDefault();
    }
}