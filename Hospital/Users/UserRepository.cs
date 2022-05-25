using MongoDB.Driver;

namespace HospitalSystem;

[System.Serializable]
public class UserDoesNotExistException : System.Exception
{
    public UserDoesNotExistException() { }
    public UserDoesNotExistException(string message) : base(message) { }
    public UserDoesNotExistException(string message, System.Exception inner) : base(message, inner) { }
    protected UserDoesNotExistException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class UserRepository
{
    private MongoClient _dbClient;

    public UserRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }

    public IMongoCollection<User> GetUsers()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<User>("users");
    }

    public void BlockUserPatient(string email)
    {
        var userToBlock = GetUser(email);
        var users = GetUsers();
        if (userToBlock.Role != Role.PATIENT)
        {
            throw new UserDoesNotExistException("User " + email + " is not a patient.");
        }
        if (userToBlock.BlockStatus != Block.UNBLOCKED){
            throw new UserDoesNotExistException("User " + email + " is already blocked.");
        }
        userToBlock.BlockStatus = Block.BY_SECRETARY;

        users.ReplaceOne(user => user.Email == email, userToBlock, new ReplaceOptions {IsUpsert = true});
    }

    public List<User> GetBlockedUsers()
    {
        var users = GetUsers();
        List<User> blockedUsers = new List<User>();
        var matchingUsers = from user in users.AsQueryable() select user;
        foreach(var p in matchingUsers){
            if (p.BlockStatus != Block.UNBLOCKED){
                blockedUsers.Add(p);
            }
        }
        return blockedUsers;
    }

    public void UnblockUserPatient(string email)
    {
        var userToUnblock = GetUser(email);
        var users = GetUsers();
        userToUnblock.BlockStatus = Block.UNBLOCKED;
        users.ReplaceOne(user => user.Email == email, userToUnblock, new ReplaceOptions {IsUpsert = true});
    }

    public User? Login(string email, string password)
    {
        var users = GetUsers();
        var matchingUsers = 
            from user in users.AsQueryable()
            where user.Password == password && user.Email == email
            select user;
        // count on database that there is only one with this email
        return matchingUsers.FirstOrDefault();
    }

    public void AddOrUpdateUser(User user)
    {
        var newUser = user;
        var users = GetUsers();
        users.ReplaceOne(user => user.Id == newUser.Id, newUser, new ReplaceOptions {IsUpsert = true});
    }

    public User GetUser(string email)
    {
        var users = GetUsers();
        var matchingUsers = 
            from user in users.AsQueryable()
            where user.Email == email
            select user;   
        if (!matchingUsers.Any()) 
            throw new UserDoesNotExistException("User " + email + " does not exist.");
        return matchingUsers.First();
    }

    public void UpdateUserPassword(string email, string newPassword)
    {
        var newUser = GetUser(email);
        var users = GetUsers();
        newUser.Password = newPassword;

        users.ReplaceOne(user => user.Email == email, newUser, new ReplaceOptions {IsUpsert = true});
    }

    public void UpdateUserEmail(string email, string newEmail)
    {
        var newUser = GetUser(email);
        var users = GetUsers();
        newUser.Email = newEmail;

        users.ReplaceOne(user => user.Email == email, newUser, new ReplaceOptions {IsUpsert = true});
    }

    public void DeleteUser(string email)
    {
        var users = GetUsers();
        var deleted = users.DeleteOne(users => users.Email == email);
        if (deleted.DeletedCount == 0)
            throw new UserDoesNotExistException("User " + email + " does not exist.");
    }
}
