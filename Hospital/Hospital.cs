 using MongoDB.Driver;

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
        public EquipmentRelocationRepository RelocationRepo { get; }
        public CheckupChangeRequestRepository CheckupChangeRequestRepo { get; }
        public SimpleRenovationRepository SimpleRenovationRepo { get; set; }
        public SplitRenovationRepository SplitRenovationRepo { get; set; }

        public Hospital()
        {
            UserRepo = new (_dbClient);
            DoctorRepo = new (_dbClient);
            PatientRepo = new (_dbClient);
            AppointmentRepo = new (_dbClient);
            DirectorRepo = new (_dbClient);
            SecretaryRepo = new (_dbClient);
            RoomRepo = new (_dbClient);
            EquipmentRepo = new (_dbClient);
            RelocationRepo = new (_dbClient, EquipmentRepo);
            CheckupChangeRequestRepo = new (_dbClient);
            SimpleRenovationRepo = new (_dbClient, RoomRepo);
            SplitRenovationRepo = new (_dbClient, RoomRepo);
        }

        public User? Login(string email, string password)
        {
            return UserRepo.Login(email, password);
        }
     }
 }  