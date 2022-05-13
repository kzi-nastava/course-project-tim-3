using MongoDB.Driver;

namespace Hospital;

public class MergeRenovationRepository
{
    private MongoClient _dbClient;
    private RoomRepository _roomRepo;  // TODO: extract to service!
    private EquipmentRelocationRepository _relocationRepo;  // TODO: extract to service!

    public MergeRenovationRepository(MongoClient dbClient, RoomRepository roomRepo, EquipmentRelocationRepository relocationRepo)
    {
        _dbClient = dbClient;
        _roomRepo = roomRepo;
        _relocationRepo = relocationRepo;
    }

    private IMongoCollection<MergeRenovation> GetCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<MergeRenovation>("merge_renovations");
    }

    public IQueryable<MergeRenovation> GetAll()
    {
        return GetCollection().AsQueryable();
    }

    public void Add(MergeRenovation renovation)
    // todo: load these on start in scheduler when making service
    {
        GetCollection().InsertOne(renovation);
    }

    // NOTE: expects existing!!
    public void Replace(MergeRenovation replacing)
    {
        GetCollection().ReplaceOne(renovation => renovation.Id == replacing.Id, replacing);
    }

    public void Schedule(MergeRenovation renovation)
    {
        Scheduler.Schedule(renovation.StartTime, () =>
        {
            _roomRepo.Deactivate(renovation.FirstLocation);
            _roomRepo.Deactivate(renovation.SecondLocation);
        });
        Scheduler.Schedule(renovation.EndTime, () => 
        {
            FinishRenovation(renovation);
        });
    }

    private void FinishRenovation(MergeRenovation renovation)
    {
        _roomRepo.Activate(renovation.MergeToLocation);
        _relocationRepo.MoveAll(renovation.FirstLocation, renovation.MergeToLocation);
        _relocationRepo.MoveAll(renovation.SecondLocation, renovation.MergeToLocation);
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