using MongoDB.Driver;

namespace HospitalSystem;

public class EquipmentRelocationRepository
{
    private MongoClient _dbClient;
    private EquipmentBatchService _equipmentService;  // TODO: extract to service!!

    public EquipmentRelocationRepository(MongoClient dbClient, EquipmentBatchService equipmentService)
    {
        _dbClient = dbClient;
        _equipmentService = equipmentService;
    }

    private IMongoCollection<EquipmentRelocation> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<EquipmentRelocation>("relocations");
    }

    public IQueryable<EquipmentRelocation> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Add(EquipmentRelocation relocation)
    {
        GetMongoCollection().InsertOne(relocation);
    }

    // NOTE: expects existing!!
    public void Replace(EquipmentRelocation replacing)
    {
        GetMongoCollection().ReplaceOne(relocation => relocation.Id == replacing.Id, replacing);
    }

    public void Schedule(EquipmentRelocation relocation)
    {
        Scheduler.Schedule(relocation.EndTime, () => 
        {
            MoveEquipment(relocation);
        });
    }

    private void MoveEquipment(EquipmentRelocation relocation)
    {
        var removing = new EquipmentBatch(relocation.FromRoomLocation, relocation.Name, relocation.Count, relocation.Type);
        var adding = new EquipmentBatch(relocation.ToRoomLocation, relocation.Name, relocation.Count, relocation.Type);
        _equipmentService.Remove(removing);
        _equipmentService.Add(adding);
        relocation.IsDone = true;
        Replace(relocation);
    }

    public void MoveAll(string fromLocation, string toLocation)
    {
        foreach (var batch in _equipmentService.GetAllIn(fromLocation))
        {
            _equipmentService.Remove(batch);
            batch.RoomLocation = toLocation;
            _equipmentService.Add(batch);
        }
    }

    // TODO: move this and some others to service
    public void ScheduleAll()
    {
        foreach (var relocation in GetAll())
        {
            if (!relocation.IsDone)
            {
                Schedule(relocation);
            }
        }
    }
}