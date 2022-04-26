using MongoDB.Driver;

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

        public User? Login(string username, string password)
        {
            var users = GetUsers();
            var matchingUsers = 
                from user in users.AsQueryable()
                where user.password == password && user.username == username
                select user;
            // count on database that there is only one with this username
            if (matchingUsers.Any()) return matchingUsers.First();
            return null;
        }

        public void AddUser(string username, string password, Role role)
        {
            var user = new User(username, password, role);
            var users = GetUsers();
            users.InsertOne(user);
        }
    }
} 
