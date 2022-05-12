using MongoDB.Driver;

namespace Hospital;

public class SplitRenovationRepository
{
    private MongoClient _dbClient;
    private RoomRepository _roomRepo;  // TODO: extract to service!

    public SplitRenovationRepository(MongoClient dbClient, RoomRepository roomRepo)
    {
        _dbClient = dbClient;
        _roomRepo = roomRepo;
    }

    private IMongoCollection<SplitRenovation> GetCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<SplitRenovation>("split_renovations");
    }

    public IQueryable<SplitRenovation> GetAll()
    {
        return GetCollection().AsQueryable();
    }

    public void Add(SplitRenovation renovation, Room firstNewRoom, Room secondNewRoom)
    // todo: load these on start in scheduler when making service
    {
        firstNewRoom.Active = false;
        secondNewRoom.Active = false;
        _roomRepo.Add(firstNewRoom);
        _roomRepo.Add(secondNewRoom);
        GetCollection().InsertOne(renovation);
    }

    // NOTE: expects existing!!
    public void Replace(SplitRenovation replacing)
    {
        GetCollection().ReplaceOne(relocation => relocation.Id == replacing.Id, replacing);
    }

    public void Schedule(SplitRenovation renovation)
    {
        // stupid way, but must be done like this (or mb better?) to avoid errors
        if (renovation.StartTime <= DateTime.Now)
        {
            _roomRepo.Deactivate(renovation.SplitRoomLocation);
        }
        else
        {
            Scheduler.Schedule(renovation.StartTime, () =>
            {
                _roomRepo.Deactivate(renovation.SplitRoomLocation);
            });
        }
        if (renovation.EndTime <= DateTime.Now)
        {
            FinishRenovation(renovation);
            _roomRepo.Delete(renovation.SplitRoomLocation);
        }
        else
        {
            Scheduler.Schedule(renovation.EndTime, () => 
            {
                FinishRenovation(renovation);
                _roomRepo.Delete(renovation.SplitRoomLocation);
            });
        }
    }

    private void FinishRenovation(SplitRenovation renovation)
    {
        renovation.IsDone = true;
        Replace(renovation);
        _roomRepo.Activate(renovation.SplitToFirstLocation);
        _roomRepo.Activate(renovation.SplitToSecondLocation);
    }

    // TODO: move this and some others to service
    public void ScheduleAll()
    {
        foreach (var renovation in GetAll())
        {
            Schedule(renovation);
        }
    }
}