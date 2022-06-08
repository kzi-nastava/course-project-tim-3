using MongoDB.Driver;

namespace HospitalSystem.Core;

public class UserRepository : IUserRepository
{
    private MongoClient _dbClient;

    public UserRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<User> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<User>("users");
    }

    public IQueryable<User> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public User Get(string email)
    {
        var users = GetAll();
        var matchingUsers = 
            from user in users
            where user.Email == email
            select user;   
        if (!matchingUsers.Any()) 
            throw new UserDoesNotExistException("User " + email + " does not exist.");
        return matchingUsers.First();
    }

    public void Upsert(User newUser)
    {
        GetMongoCollection().ReplaceOne(user => user.Id == newUser.Id, newUser,
            new ReplaceOptions {IsUpsert = true});
    }

    public bool Delete(string email)
    {
        return GetMongoCollection().DeleteOne(user => user.Email == email).DeletedCount == 1;
    }

    public IQueryable<User> GetPatients()
    {
        return 
            from user in GetAll()
            where user.Role == Role.PATIENT
            select user;
    }
}