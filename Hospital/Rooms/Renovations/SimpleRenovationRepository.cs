using MongoDB.Driver;

namespace Hospital;

public class SimpleRenovationRepository
{
    private MongoClient _dbClient;
    private RoomRepository _roomRepo;  // TODO: extract to service!

    public SimpleRenovationRepository(MongoClient dbClient, RoomRepository roomRepo)
    {
        _dbClient = dbClient;
        _roomRepo = roomRepo;
    }

    private IMongoCollection<SimpleRenovation> GetCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<SimpleRenovation>("simple_renovations");
    }

    public IQueryable<SimpleRenovation> GetAll()
    {
        return GetCollection().AsQueryable();
    }

    public void Add(SimpleRenovation renovation)
    // todo: load these on start in scheduler when making service
    {
        GetCollection().InsertOne(renovation);
    }

    // NOTE: expects existing!!
    public void Replace(SimpleRenovation replacing)
    {
        GetCollection().ReplaceOne(relocation => relocation.Id == replacing.Id, replacing);
    }

    public void Schedule(SimpleRenovation renovation)
    {
        // stupid way, but must be done like this (or mb better?) to avoid errors
        if (renovation.StartTime <= DateTime.Now)
        {
            _roomRepo.Deactivate(renovation.RoomLocation);
        }
        else
        {
            Scheduler.Schedule(renovation.StartTime, () =>
            {
                _roomRepo.Deactivate(renovation.RoomLocation);
            });
        }
        if (renovation.EndTime <= DateTime.Now)
        {
            FinishRenovation(renovation);
        }
        else
        {
            Scheduler.Schedule(renovation.EndTime, () => 
            {
                FinishRenovation(renovation);
            });
        }
    }

    private void FinishRenovation(SimpleRenovation renovation)
    {
        renovation.IsDone = true;
        Replace(renovation);
        _roomRepo.Activate(renovation.RoomLocation);
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