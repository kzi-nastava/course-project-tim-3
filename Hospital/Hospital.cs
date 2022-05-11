 using MongoDB.Driver;
 using MongoDB.Bson;

 namespace Hospital
 {
     public class Hospital
     {
        private MongoClient _dbClient = new MongoClient("mongodb://root:root@localhost:27017"); // TODO: move this
        public UserRepository UserRepo {get;}
        public DoctorRepository DoctorRepo {get;}
        public PatientRepository PatientRepo {get;}
        public AppointmentRepository AppointmentRepo {get;}
        public DirectorRepository DirectorRepo {get;}
        public SecretaryRepository SecretaryRepo {get;}
        public RoomRepository RoomRepo {get;}
        public EquipmentBatchRepository EquipmentRepo { get; }
        public CheckupChangeRequestRepository CheckupChangeRequestRepo { get; }

        public Hospital()
        {
            UserRepo = new UserRepository(_dbClient);
            DoctorRepo = new DoctorRepository(_dbClient);
            PatientRepo = new PatientRepository(_dbClient);
            AppointmentRepo = new AppointmentRepository(_dbClient);
            DirectorRepo = new DirectorRepository(_dbClient);
            SecretaryRepo = new SecretaryRepository(_dbClient);
            RoomRepo = new RoomRepository(_dbClient);
            EquipmentRepo = new EquipmentBatchRepository(_dbClient);
            CheckupChangeRequestRepo = new CheckupChangeRequestRepository(_dbClient);
        }

        public User? Login(string email, string password)
        {
            return UserRepo.Login(email, password);
        }
     }
 }  