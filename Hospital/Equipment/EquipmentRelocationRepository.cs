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
    public void DeleteRelocation(ObjectId deletingId)
    {
        GetEquipmentRelocations().DeleteOne(relocation => relocation.Id == deletingId);
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
        var fromBatch = _equipmentRepo.GetEquipmentBatch((ObjectId) relocation.FromRoom.Id, relocation.Name);
        var addingBatch = new EquipmentBatch((ObjectId) relocation.ToRoom.Id, relocation.Name, relocation.Count, relocation.Type);
        var toBatch = _equipmentRepo.GetEquipmentBatch((ObjectId) relocation.ToRoom.Id, relocation.Name);
        _equipmentRepo.RemoveEquipmentBatch(removingBatch);
        _equipmentRepo.AddEquipmentBatch(addingBatch);
        DeleteRelocation(relocation.Id);
    }
}