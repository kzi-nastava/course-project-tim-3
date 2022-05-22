using MongoDB.Driver;
using HospitalSystem.Utils;

namespace HospitalSystem;

public class MergeRenovationRepository
{
    private MongoClient _dbClient;
    private RoomRepository _roomRepo;  // TODO: extract to service!
    private EquipmentRelocationService _relocationService;

    public MergeRenovationRepository(MongoClient dbClient, RoomRepository roomRepo,
        EquipmentRelocationService relocationService)
    {
        _dbClient = dbClient;
        _roomRepo = roomRepo;
        _relocationService = relocationService;
    }

    private IMongoCollection<MergeRenovation> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<MergeRenovation>("merge_renovations");
    }

    public IQueryable<MergeRenovation> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Add(MergeRenovation renovation)
    {
        GetMongoCollection().InsertOne(renovation);
    }

    // NOTE: expects existing!!
    public void Replace(MergeRenovation replacing)
    {
        GetMongoCollection().ReplaceOne(renovation => renovation.Id == replacing.Id, replacing);
    }

    public void Schedule(MergeRenovation renovation)
    {
        Scheduler.Schedule(renovation.BusyRange.Starts, () =>
        {
            _roomRepo.Deactivate(renovation.FirstLocation);
            _roomRepo.Deactivate(renovation.SecondLocation);
        });
        Scheduler.Schedule(renovation.BusyRange.Ends, () => 
        {
            FinishRenovation(renovation);
        });
    }

    private void FinishRenovation(MergeRenovation renovation)
    {
        _roomRepo.Activate(renovation.MergeToLocation);
        _relocationService.MoveAll(renovation.FirstLocation, renovation.MergeToLocation);
        _relocationService.MoveAll(renovation.SecondLocation, renovation.MergeToLocation);
        _roomRepo.Delete(renovation.FirstLocation);
        _roomRepo.Delete(renovation.SecondLocation);
        renovation.IsDone = true;
        Replace(renovation);
    }

    // TODO: move this and some others to service
    public void ScheduleAll()
    {
        foreach (var renovation in GetAll())
        {
            if (!renovation.IsDone)
            {
                Schedule(renovation);
            }
        }
    }
}