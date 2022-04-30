 using MongoDB.Driver;
 using MongoDB.Bson;

 namespace Hospital
 {
     public class Hospital
     {
        private MongoClient _dbClient = new MongoClient("mongodb://root:root@localhost:27017"); // TODO: move this
        public UserRepository UserRepo {get;}
        
        public DoctorRepository DoctorRepo {get;}

        public AppointmentRepository AppointmentRepo {get;}

        public PatientRepository PatientRepo {get;}

        public Hospital()
        {
            UserRepo = new UserRepository(_dbClient);
            DoctorRepo = new DoctorRepository(_dbClient);
            AppointmentRepo = new AppointmentRepository(_dbClient);
            PatientRepo = new PatientRepository(_dbClient);
        }

        public User? Login(string email, string password)
        {
            return UserRepo.Login(email, password);
        }
     }
 }  