using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace HospitalSystem;

public class EquipmentBatchRepository
{
    private MongoClient _dbClient;

    public EquipmentBatchRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }

    private IMongoCollection<EquipmentBatch> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<EquipmentBatch>("equipment");
    }

    public IQueryable<EquipmentBatch> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public IQueryable<EquipmentBatch> GetAllIn(string roomLocation)
    {
        var batches = GetAll();
        var matches = 
            from batch in batches
            where batch.RoomLocation == roomLocation
            select batch;
        return matches;
    }

    public void Add(EquipmentBatch newBatch)
    {
        var batch = Get(newBatch.RoomLocation, newBatch.Name);
        if (batch is null)
        {
            GetMongoCollection().InsertOne(newBatch);
        }
        else
        {
            batch.MergeWith(newBatch);
            Replace(batch);
        }
    }

    // NOTE: only use during Relocation!!
    public void Remove(EquipmentBatch removingBatch)
    {
        var existingBatch = Get(removingBatch.RoomLocation, removingBatch.Name);
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
                GetMongoCollection().DeleteOne(batch => batch.Id == existingBatch.Id);
        }
    }

    public void DeleteAllInRoom(Room room)
    {
        GetMongoCollection().DeleteMany(batch => batch.RoomLocation == room.Location);
    }

    public void Replace(EquipmentBatch newBatch) // EXPECTS EXISTING EQUIPMENTBATCH!
    {
        GetMongoCollection().ReplaceOne(batch => batch.Id == newBatch.Id, newBatch);
    }

    public EquipmentBatch? Get(string roomLocation, string name)
    {
        var batches = GetMongoCollection();
        return batches.Find(batch => batch.RoomLocation == roomLocation && batch.Name == name).FirstOrDefault();
    }

    public IQueryable<EquipmentBatch> Search(EquipmentQuery query)  // TODO: probably have to move this
    {
        var batches = GetAll();
        var matches = 
            from batch in batches
            where (query.MinCount == null || query.MinCount <= batch.Count)
                && (query.MaxCount == null || query.MaxCount >= batch.Count)
                && (query.Type == null || query.Type == batch.Type)
                && (query.NameContains == null || query.NameContains.IsMatch(batch.Name))
            select batch;
        return matches;
    }
}