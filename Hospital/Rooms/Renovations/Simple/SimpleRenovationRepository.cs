using MongoDB.Driver;

namespace HospitalSystem;

public class SimpleRenovationRepository : ISimpleRenovationRepository
{
    private MongoClient _dbClient;

    public SimpleRenovationRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<SimpleRenovation> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<SimpleRenovation>("simple_renovations");
    }

    public IQueryable<SimpleRenovation> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Replace(SimpleRenovation replacing)
    {
        GetMongoCollection().ReplaceOne(renovation => renovation.Id == replacing.Id, replacing);
    }

    public void Insert(SimpleRenovation renovation)
    {
        GetMongoCollection().InsertOne(renovation);
    }
}