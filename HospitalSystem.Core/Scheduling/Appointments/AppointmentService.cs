using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core.Rooms;

namespace HospitalSystem.Core;

public class AppointmentService
{
    private IAppointmentRepository _appointmentRepo;
    private RoomService _roomService;
    private DoctorService _doctorService;
    private PatientService _patientService;

    public AppointmentService(IAppointmentRepository appointmentRepo, RoomService roomService, DoctorService doctorService, PatientService patientService)
    {
        _appointmentRepo = appointmentRepo;
        _roomService = roomService;
        _doctorService = doctorService;
        _patientService = patientService;
    }

    public void UpsertCheckup(Checkup newCheckup)
    {
        _appointmentRepo.UpsertCheckup(newCheckup);
    }
    public void UpsertOperation(Operation newOperation)
    {
        _appointmentRepo.UpsertOperation(newOperation);
    }

    public void DeleteCheckup(Checkup checkup)
    {
        _appointmentRepo.DeleteCheckup(checkup);
    }

    public void DeleteOperation(Operation operation)
    {
        _appointmentRepo.DeleteOperation(operation);
    }

    public void FinishCheckup(Checkup checkup)
    {
        checkup.Done = true;
        _appointmentRepo.UpsertCheckup(checkup);
    }

     public IMongoCollection<Checkup> GetCheckups()
    {
        return _appointmentRepo.GetCheckups();
    }

    public IMongoCollection<Operation> GetOperations()
    {
        return _appointmentRepo.GetOperations();
    }
    public List<Checkup> GetCheckupsByPatient(ObjectId id)
    {
        var checkups = _appointmentRepo.GetCheckups();
        List<Checkup> patientCheckups = checkups.Find(appointment => appointment.Patient.Id == id).ToList();
        return patientCheckups;
    }
    
    public List<Checkup> SearchPastCheckups(ObjectId patientId, string anamnesisKeyword)
    {
        var checkups = _appointmentRepo.GetCheckups();
        //might not be the best way to indent
        List<Checkup> filteredCheckups = 
            (from checkup in checkups.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where checkup.Anamnesis.ToLower().Contains(anamnesisKeyword.ToLower())
            && checkup.DateRange.HasPassed() 
            && checkup.Patient.Id == patientId
            select checkup).ToList();
        return filteredCheckups;
    }

    public List<Checkup> GetFutureCheckupsByPatient(ObjectId id)
    {
        var checkups = _appointmentRepo.GetCheckups();
        List<Checkup> patientCheckups = 
            (from checkup in checkups.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where checkup.DateRange.IsFuture() && checkup.Patient.Id == id
            select checkup).ToList();
        return patientCheckups;
    }

    public List<Operation> GetFutureOperationsByPatient(ObjectId id)
    {
        var operations = _appointmentRepo.GetOperations();
        List<Operation> patientOperations = 
            (from operation in operations.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where operation.DateRange.IsFuture() && operation.Patient.Id == id
            select operation).ToList();
        return patientOperations;
    }

    public List<Operation> GetOperationsByPatient(ObjectId id)
    {
        return _appointmentRepo.GetOperationsByPatient(id);
    }

    public List<Operation> GetOperationsByDoctor(ObjectId id)
    {
       return _appointmentRepo.GetOperationsByDoctor(id);
    }

    public List<Checkup> GetCheckupsByDay(DateTime date)
    {
        var checkups = _appointmentRepo.GetCheckups();
        List<Checkup> checkupsByDay = 
            (from checkup in checkups.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where checkup.DateRange.Starts.Date == date.Date
            select checkup).ToList();
        return checkupsByDay;
    }

    public List<Checkup> GetCheckupSchedule(Doctor doctor, DateTime day)
    {
        List<Checkup> checkupsByDay = new();
        foreach (Checkup checkup in GetCheckupsByDay(day))
        {
            if (checkup.Doctor.Id == doctor.Id)
                checkupsByDay.Add(checkup);
        }
        return checkupsByDay;
    }

    public List<Checkup> GetNotDoneCheckups(Doctor doctor, DateTime day)
    {
        var checkups = GetCheckupSchedule(doctor, day);
        List<Checkup> checkupsByDay = 
            (from checkup in checkups.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where checkup.DateRange.Starts.Date == day.Date && checkup.Done == false
            select checkup).ToList();
        return checkupsByDay;
    }

    public List<Operation> GetOperationsByDay(DateTime date)
    {
        var operations = _appointmentRepo.GetOperations();
        List<Operation> operationsByDay = 
            (from operation in operations.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where operation.DateRange.Starts.Date == date.Date
            select operation).ToList();
        return operationsByDay;
    }

    public List<Operation> GetOperationSchedule(Doctor doctor, DateTime day)
    {
        List<Operation> operationsByDay = new();
        foreach (Operation operation in GetOperationsByDay(day))
        {
            if (operation.Doctor.Id == doctor.Id)
                operationsByDay.Add(operation);
        }
        return operationsByDay;
    }

    public List<Operation> GetNotDoneOperations(Doctor doctor, DateTime day)
    {
        var operations = GetOperationSchedule(doctor, day);
        List<Operation> operationsByDay = 
            (from operation in operations.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where operation.DateRange.Starts.Date == day.Date && operation.Done == false
            select operation).ToList();
        return operationsByDay;
    }

    public List<Checkup> GetPastCheckupsByPatient(ObjectId id)
    {
        var checkups = _appointmentRepo.GetCheckups();
        List<Checkup> selectedCheckups = 
            (from checkup in checkups.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where checkup.DateRange.HasPassed() && checkup.Patient.Id == id
            select checkup).ToList();
        return selectedCheckups;
    }

    public List<Operation> GetPastOperationsByPatient(ObjectId id)
    {
        var operations = _appointmentRepo.GetOperations();
        List<Operation> patientOperations = 
            (from operation in operations.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where operation.DateRange.HasPassed() && operation.Patient.Id == id
            select operation).ToList();
        return patientOperations;
    }
    
    public List<Checkup> GetCheckupsByDoctor(Doctor doctor)
    {
        return _appointmentRepo.GetCheckupsByDoctor(doctor);
    }

    public float GetAverageRating(Doctor doctor)
    {
        // TODO: instead of doing this, move to SurveyService, and there get Drs together with ratings
        throw new NotImplementedException("Don't like this, read todo in line above me");
    }

    public HashSet<ObjectId> GetAllAppointmentDoctors(Patient pat)
    {
        return _appointmentRepo.GetAllAppointmentDoctors(pat);
    }
}