using MongoDB.Driver;
using HospitalSystem.Utils;

namespace HospitalSystem;

public class MergeRenovationRepository
{
    private MongoClient _dbClient;
    private RoomService _roomService;
    private EquipmentRelocationService _relocationService;

    public MergeRenovationRepository(MongoClient dbClient, RoomService roomService,
        EquipmentRelocationService relocationService)
    {
        _dbClient = dbClient;
        _roomService = roomService;
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
            _roomService.Deactivate(renovation.FirstLocation);
            _roomService.Deactivate(renovation.SecondLocation);
        });
        Scheduler.Schedule(renovation.BusyRange.Ends, () => 
        {
            FinishRenovation(renovation);
        });
    }

    private void FinishRenovation(MergeRenovation renovation)
    {
        _roomService.Activate(renovation.MergeToLocation);
        _relocationService.MoveAll(renovation.FirstLocation, renovation.MergeToLocation);
        _relocationService.MoveAll(renovation.SecondLocation, renovation.MergeToLocation);
        _roomService.Delete(renovation.FirstLocation);
        _roomService.Delete(renovation.SecondLocation);
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