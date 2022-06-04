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
    public DoctorService DoctorService { get; }
    public RenovationService RenovationService { get; }
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
        PatientService = new (PatientRepo);
        // TODO : Might be a wrong way to create a service
        DoctorService = new (DoctorRepo);
        AppointmentService = new (AppointmentRepo, RoomService, DoctorService);
        EquipmentService = new (new EquipmentBatchRepository(_dbClient));
        RelocationService = new (new EquipmentRelocationRepository(_dbClient), EquipmentService);
        CheckupChangeRequestRepo = new (_dbClient);
        RenovationService = new (new RenovationRepository(_dbClient), RoomService,
            RelocationService, AppointmentService);
        MedicationRepo = new (_dbClient);
        MedicationRequestService = new (new MedicationRequestRepository(_dbClient), MedicationRepo);
        
        // TODO: this maybe shouldn't be here
        RelocationService.ScheduleAll();
        RenovationService.ScheduleAll();
    }
}