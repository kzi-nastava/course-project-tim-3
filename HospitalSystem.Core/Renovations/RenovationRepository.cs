using MongoDB.Driver;

namespace HospitalSystem.Core;

public class RenovationRepository : IRenovationRepository
{
    private MongoClient _dbClient;

    public RenovationRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<Renovation> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Renovation>("renovations");
    }

    public IQueryable<Renovation> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Insert(Renovation renovation)
    {
        GetMongoCollection().InsertOne(renovation);
    }

    public void Replace(Renovation replacement)
    {
        GetMongoCollection().ReplaceOne(renovation => renovation.Id == replacement.Id, replacement);
    }
}