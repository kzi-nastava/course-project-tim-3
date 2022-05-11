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

    private IMongoCollection<EquipmentRelocation> GetCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<EquipmentRelocation>("relocations");
    }

    public IQueryable<EquipmentRelocation> GetAll()
    {
        return GetCollection().AsQueryable();
    }

    public void Add(EquipmentRelocation relocation)
    // todo: load these on start in scheduler
    {
        GetCollection().InsertOne(relocation);
    }

    // NOTE: expects existing!!
    public void Replace(EquipmentRelocation replacing)
    {
        GetCollection().ReplaceOne(relocation => relocation.Id == replacing.Id, replacing);
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
        var removing = new EquipmentBatch(relocation.FromRoomLocation, relocation.Name, relocation.Count, relocation.Type);
        var adding = new EquipmentBatch(relocation.ToRoomLocation, relocation.Name, relocation.Count, relocation.Type);
        _equipmentRepo.Remove(removing);
        _equipmentRepo.Add(adding);
        relocation.IsDone = true;
        Replace(relocation);
    }

    // TODO: move this and some others to service
    public void ScheduleAll()
    {
        foreach (var relocation in GetAll())
        {
            Schedule(relocation);
        }
    }
}