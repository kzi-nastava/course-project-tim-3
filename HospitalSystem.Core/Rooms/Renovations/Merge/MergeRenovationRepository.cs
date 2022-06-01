using MongoDB.Driver;

namespace HospitalSystem.Core;

public class MergeRenovationRepository : IMergeRenovationRepository
{
    private MongoClient _dbClient;

    public MergeRenovationRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<MergeRenovation> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<MergeRenovation>("merge_renovations");
    }

    public IQueryable<MergeRenovation> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Insert(MergeRenovation renovation)
    {
        GetMongoCollection().InsertOne(renovation);
    }

    public void Replace(MergeRenovation replacing)
    {
        GetMongoCollection().ReplaceOne(renovation => renovation.Id == replacing.Id, replacing);
    }
}