using MongoDB.Driver;

namespace HospitalSystem.Core;

public class SplitRenovationRepository : ISplitRenovationRepository
{
    private MongoClient _dbClient;

    public SplitRenovationRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<SplitRenovation> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<SplitRenovation>("split_renovations");
    }

    public IQueryable<SplitRenovation> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Insert(SplitRenovation renovation)
    {
        GetMongoCollection().InsertOne(renovation);
    }

    public void Replace(SplitRenovation replacing)
    {
        GetMongoCollection().ReplaceOne(renovation => renovation.Id == replacing.Id, replacing);
    }
}