using MongoDB.Driver;
using HospitalSystem.Core.Surveys;
using HospitalSystem.Core.Renovations;
using HospitalSystem.Core.Rooms;

namespace HospitalSystem.Core;

public class Hospital
{
    private MongoClient _dbClient = new MongoClient("mongodb://root:root@localhost:27017"); // TODO: move this
    public UserService UserService { get; }
    public DirectorRepository DirectorRepo { get; }
    public SecretaryRepository SecretaryRepo { get; }
    public RoomService RoomService { get; }
    public EquipmentBatchService EquipmentService { get; }
    public EquipmentOrderService EquipmentOrderService { get; }
    public EquipmentRelocationService RelocationService { get; }
    public DoctorService DoctorService { get; }
    public RenovationService RenovationService { get; }
    public MedicationRepository MedicationRepo { get; }
    public MedicationRequestService MedicationRequestService { get; }
    public AppointmentService AppointmentService { get; }
    public PatientService PatientService { get; }
    public CheckupChangeRequestService CheckupChangeRequestService  { get; }
    public SurveyService SurveyService { get; set; }

    public Hospital()
    {
        UserService = new (new UserRepository(_dbClient));
        DirectorRepo = new (_dbClient);
        SecretaryRepo = new (_dbClient);
        RoomService = new (new RoomRepository(_dbClient));
        // TODO : Might be a wrong way to create a service
        PatientService = new (new PatientRepository(_dbClient));
        // TODO : Might be a wrong way to create a service
        DoctorService = new (new DoctorRepository(_dbClient));
        AppointmentService = new (new AppointmentRepository(_dbClient), RoomService, DoctorService, PatientService);
        EquipmentService = new (new EquipmentBatchRepository(_dbClient));
        EquipmentOrderService = new (new EquipmentOrderRepository(_dbClient), EquipmentService); 
        RelocationService = new (new EquipmentRelocationRepository(_dbClient), EquipmentService);
        CheckupChangeRequestService = new (new CheckupChangeRequestRepository(_dbClient));
        RenovationService = new (new RenovationRepository(_dbClient), RoomService,
        RelocationService, AppointmentService);
        MedicationRepo = new (_dbClient);
        MedicationRequestService = new (new MedicationRequestRepository(_dbClient), MedicationRepo);
        SurveyService = new SurveyService(new SurveyRepository(_dbClient),
            AppointmentService, DoctorService);
        
        // TODO: this maybe shouldn't be here
        RelocationService.ScheduleAll();
        RenovationService.ScheduleAll();
        EquipmentOrderService.ScheduleAll();
    }
}