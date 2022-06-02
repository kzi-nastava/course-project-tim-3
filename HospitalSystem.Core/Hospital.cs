using MongoDB.Driver;

namespace HospitalSystem.Core;

public class Hospital
{
    private MongoClient _dbClient = new MongoClient("mongodb://root:root@localhost:27017"); // TODO: move this
    public UserService UserService { get; }
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
    public MergeRenovationService MergeRenovationService { get; }
    public MedicationRepository MedicationRepo { get; }
    public MedicationRequestService MedicationRequestService { get; }
    public AppointmentService AppointmentService { get; }
     public PatientService PatientService { get; }

    public Hospital()
    {
        UserService = new (new UserRepository(_dbClient));
        DoctorRepo = new (_dbClient);
        PatientRepo = new (_dbClient);
        DirectorRepo = new (_dbClient);
        SecretaryRepo = new (_dbClient);
        RoomService = new (new RoomRepository(_dbClient));
        AppointmentRepo = new (_dbClient);
        // TODO : Might be a wrong way to create a service
        PatientService = new (new PatientRepository(_dbClient));
        // TODO : Might be a wrong way to create a service
        AppointmentService = new (new AppointmentRepository(_dbClient), RoomService, DoctorRepo);
        EquipmentService = new (new EquipmentBatchRepository(_dbClient));
        RelocationService = new (new EquipmentRelocationRepository(_dbClient), EquipmentService);
        CheckupChangeRequestRepo = new (_dbClient);
        SimpleRenovationService = new (new SimpleRenovationRepository(_dbClient), RoomService, AppointmentService);
        SplitRenovationService = new (new SplitRenovationRepository(_dbClient), RoomService,
            RelocationService, AppointmentService);
        MergeRenovationService = new (new MergeRenovationRepository(_dbClient), RoomService,
            RelocationService, AppointmentService);
        MedicationRepo = new (_dbClient);
        MedicationRequestService = new (new MedicationRequestRepository(_dbClient), MedicationRepo);
        
        // TODO: this maybe shouldn't be here
        RelocationService.ScheduleAll();
        SimpleRenovationService.ScheduleAll();
        SplitRenovationService.ScheduleAll();
        MergeRenovationService.ScheduleAll();

    }
}