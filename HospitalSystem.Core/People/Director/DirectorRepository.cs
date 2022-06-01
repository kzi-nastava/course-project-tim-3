using MongoDB.Driver;

namespace HospitalSystem.Core;
public class DirectorRepository
{
    private MongoClient _dbClient;

    public DirectorRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }

    private IMongoCollection<Director> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Director>("directors");
    }

    public void AddOrUpdateDirector(Director newDirector)
    {
        var directors = GetMongoCollection();
        directors.ReplaceOne(director => director.Id == newDirector.Id, newDirector, new ReplaceOptions {IsUpsert = true});
    }
}
