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
        var batches = GetAll();
        var matches = 
            from batch in batches
            where batch.RoomLocation == room.Location
            select batch;
        return matches;
    }

    public void Add(EquipmentBatch newBatch)
    {
        var batch = Get(newBatch.RoomLocation, newBatch.Name);
        if (batch is null)
        {
            GetCollection().InsertOne(newBatch);
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
                GetCollection().DeleteOne(batch => batch.Id == existingBatch.Id);
        }
    }

    public void DeleteInRoom(Room room)
    {
        GetCollection().DeleteMany(batch => batch.RoomLocation == room.Location);
    }

    private void Replace(EquipmentBatch newBatch) // EXPECTS EXISTING EQUIPMENTBATCH!
    {
        GetCollection().ReplaceOne(batch => batch.Id == newBatch.Id, newBatch);
    }

    public EquipmentBatch? Get(string roomLocation, string name)
    {
        var batches = GetCollection();
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