 using MongoDB.Driver;
 using MongoDB.Bson;
 namespace Hospital
 {
     class Hospital
     {
        private MongoClient _dbClient = new MongoClient("mongodb://root:root@localhost:27017"); // TODO: move this

        private IMongoCollection<User> GetUsers()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<User>("users");
        }

        public User? Login(string username, string password)
        {
            var users = GetUsers();
            var query = 
                from user in users.AsQueryable()
                where user.password == password && user.username == username
                select user;
            // count on database that there is only one with this username
            if (query.Any()) return query.First();
            return null;
        }

        public void AddUser(string username, string password, Role role) {
            var user = new User(username, password, role);
            var users = GetUsers();
            users.InsertOne(user);
        }
     }
 }