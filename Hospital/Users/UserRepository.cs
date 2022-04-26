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
    }
} 
