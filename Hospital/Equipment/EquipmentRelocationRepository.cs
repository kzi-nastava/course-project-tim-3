using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace Hospital;

public class EquipmentRelocationRepository
{
    private MongoClient _dbClient;
    private EquipmentBatchRepository _equipmentRepo;  // TODO: extract to service!!

    public EquipmentRelocationRepository(MongoClient dbClient, EquipmentBatchRepository equipmentRepo)
    {
        _dbClient = dbClient;
        _equipmentRepo = equipmentRepo;
    }

    public IMongoCollection<EquipmentRelocation> GetEquipmentRelocations()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<EquipmentRelocation>("relocations");
    }

    public IMongoQueryable<EquipmentRelocation> GetQueryableEquipmentRelocations()
    {
        return GetEquipmentRelocations().AsQueryable();
    }

    public void AddRelocation(EquipmentRelocation relocation)
    // todo: load these on start in scheduler
    {
        GetEquipmentRelocations().InsertOne(relocation);
    }

    // NOTE: expects existing!!
    public void UpdateRelocation(EquipmentRelocation replacing)
    {
        GetEquipmentRelocations().ReplaceOne(relocation => relocation.Id == replacing.Id, replacing);
    }

    public void Schedule(EquipmentRelocation relocation)
    {
        Scheduler.Schedule(relocation.WhenDone, () => 
        {
            MoveEquipment(relocation);
        });
    }

    private void MoveEquipment(EquipmentRelocation relocation)
    {
        var removingBatch = new EquipmentBatch((ObjectId) relocation.FromRoom.Id, relocation.Name, relocation.Count, relocation.Type);
        var addingBatch = new EquipmentBatch((ObjectId) relocation.ToRoom.Id, relocation.Name, relocation.Count, relocation.Type);
        _equipmentRepo.Remove(removingBatch);
        _equipmentRepo.Add(addingBatch);
        relocation.IsDone = true;
        UpdateRelocation(relocation);
    }
}