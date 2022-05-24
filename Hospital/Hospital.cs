using MongoDB.Driver;

namespace HospitalSystem;

public class Hospital
{
    private MongoClient _dbClient = new MongoClient("mongodb://root:root@localhost:27017"); // TODO: move this
    public UserRepository UserRepo { get; }
    public DoctorRepository DoctorRepo { get; }
    public PatientRepository PatientRepo { get; }
    public AppointmentRepository AppointmentRepo { get; }
    public DirectorRepository DirectorRepo { get; }
    public SecretaryRepository SecretaryRepo { get; }
    public RoomService RoomService { get; }
    public EquipmentBatchService EquipmentService { get; }
    public EquipmentRelocationService RelocationService { get; }
    public CheckupChangeRequestRepository CheckupChangeRequestRepo { get; }
    public SimpleRenovationService SimpleRenovationService { get; }
    public SplitRenovationService SplitRenovationService { get; }
    public MergeRenovationRepository MergeRenovationRepo { get; }
    public MedicationRepository MedicationRepo {get; set;}

    public Hospital()
    {
        UserRepo = new (_dbClient);
        DoctorRepo = new (_dbClient);
        PatientRepo = new (_dbClient);
        DirectorRepo = new (_dbClient);
        SecretaryRepo = new (_dbClient);
        RoomService = new (new RoomRepository(_dbClient));
        AppointmentRepo = new (_dbClient, RoomService);
        EquipmentService = new (new EquipmentBatchRepository(_dbClient));
        RelocationService = new (new EquipmentRelocationRepository(_dbClient), EquipmentService);
        CheckupChangeRequestRepo = new (_dbClient);
        SimpleRenovationService = new (new SimpleRenovationRepository(_dbClient), RoomService);
        SplitRenovationService = new (new SplitRenovationRepository(_dbClient), RoomService, RelocationService);
        MergeRenovationRepo = new (_dbClient, RoomService, RelocationService);
        MedicationRepo = new (_dbClient);
    }

    public User? Login(string email, string password)
    {
        return UserRepo.Login(email, password);
    }
}