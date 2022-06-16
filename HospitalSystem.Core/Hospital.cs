using MongoDB.Driver;
using HospitalSystem.Core.Surveys;
using HospitalSystem.Core.Rooms;
using HospitalSystem.Core.Equipment;
using HospitalSystem.Core.Equipment.Relocations;
using HospitalSystem.Core.Renovations;
using HospitalSystem.Core.Medications;
using HospitalSystem.Core.Medications.Requests;
using HospitalSystem.Core.DaysOff;


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
    public DaysOffRequestService DaysOffRequestService  { get; }
    public HospitalSurveyService HospitalSurveyService { get; }
    public DoctorSurveyService DoctorSurveyService { get; }

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
        DaysOffRequestService = new (new DaysOffRepository(_dbClient), AppointmentService);
        HospitalSurveyService = new HospitalSurveyService(new HospitalSurveyRepository(_dbClient));
        DoctorSurveyService = new DoctorSurveyService(new DoctorSurveyRepository(_dbClient),
            AppointmentService, DoctorService);
        
        // TODO: this maybe shouldn't be here
        RelocationService.ScheduleAll();
        RenovationService.ScheduleAll();
        EquipmentOrderService.ScheduleAll();
    }
}