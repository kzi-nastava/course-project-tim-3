using MongoDB.Driver;
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

namespace Hospital
{
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

        public User? Login(string email, string password)
        {
            var users = GetUsers();
            var matchingUsers = 
                from user in users.AsQueryable()
                where user.Password == password && user.Email == email
                select user;
            // count on database that there is only one with this email
            if (matchingUsers.Any()) return matchingUsers.First();
            return null;
        }

        public void AddUser(string email, string password, string firstName, string lastName, Role role)
        {
            var newUser = new User(email, password, firstName, lastName, role);
            var users = GetUsers();
            users.ReplaceOne(user => user.Email == newUser.Email, newUser, new ReplaceOptions {IsUpsert = true});
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

        public void DeleteUser(string email)
        {
            var users = GetUsers();
            var deleted = users.DeleteOne(users => users.Email == email);
            if (deleted.DeletedCount == 0)
                throw new UserDoesNotExistException("User " + email + " does not exist.");
        }
    }
} 
