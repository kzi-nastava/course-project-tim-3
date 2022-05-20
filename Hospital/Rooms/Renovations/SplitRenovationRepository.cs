using MongoDB.Driver;

namespace HospitalSystem;

public class SplitRenovationRepository
{
    private MongoClient _dbClient;
    private RoomRepository _roomRepo;  // TODO: extract to service!
    private EquipmentRelocationRepository _relocationRepo;  // TODO: extract to service!

    public SplitRenovationRepository(MongoClient dbClient, RoomRepository roomRepo, EquipmentRelocationRepository relocationRepo)
    {
        _dbClient = dbClient;
        _roomRepo = roomRepo;
        _relocationRepo = relocationRepo;
    }

    private IMongoCollection<SplitRenovation> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<SplitRenovation>("split_renovations");
    }

    public IQueryable<SplitRenovation> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Add(SplitRenovation renovation)
    // todo: load these on start in scheduler when making service
    {
        GetMongoCollection().InsertOne(renovation);
    }

    // NOTE: expects existing!!
    public void Replace(SplitRenovation replacing)
    {
        GetMongoCollection().ReplaceOne(renovation => renovation.Id == replacing.Id, replacing);
    }

    public void Schedule(SplitRenovation renovation)
    {
        Scheduler.Schedule(renovation.BusyRange.Starts, () =>
        {
            _roomRepo.Deactivate(renovation.SplitRoomLocation);
        });
        Scheduler.Schedule(renovation.BusyRange.Ends, () => 
        {
            FinishRenovation(renovation);
        });
    }

    private void FinishRenovation(SplitRenovation renovation)
    {
        _roomRepo.Activate(renovation.SplitToFirstLocation);
        _roomRepo.Activate(renovation.SplitToSecondLocation);
        _relocationRepo.MoveAll(renovation.SplitRoomLocation, renovation.SplitToFirstLocation);
        _roomRepo.Delete(renovation.SplitRoomLocation);
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