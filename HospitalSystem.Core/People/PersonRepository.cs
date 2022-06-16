using MongoDB.Driver;

namespace HospitalSystem.Core;

public class PersonRepository : IPersonRepository
{
    private MongoClient _dbClient;

    public PersonRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<Person> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Person>("people");
    }

    public void Insert(Person person)
    {
        GetMongoCollection().InsertOne(person);
    }
}