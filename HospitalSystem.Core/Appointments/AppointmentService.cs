using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

public class AppointmentService
{
    private IAppointmentRepository _appointmentRepo;
    private RoomService _roomService;
    private DoctorRepository _doctorRepo;

    public AppointmentService(AppointmentRepository appointmentRepo, RoomService roomService, DoctorRepository doctorRepo)
    {
        _appointmentRepo = appointmentRepo;
        _roomService = roomService;
        _doctorRepo = doctorRepo;
    }

    public void AddOrUpdateCheckup(Checkup newCheckup)
    {
        newCheckup.RoomLocation = GetAvailableRoom(newCheckup, RoomType.CHECKUP).Location;
        _appointmentRepo.AddOrUpdateCheckup(newCheckup);
    }

    public void AddOrUpdateOperation(Operation newOperation)
    {
        newOperation.RoomLocation = GetAvailableRoom(newOperation, RoomType.OPERATION).Location;
        _appointmentRepo.AddOrUpdateOperation(newOperation);
    }

     public void DeleteCheckup(Checkup checkup)
    {
        _appointmentRepo.DeleteCheckup(checkup);
    }

    private Room GetAvailableRoom(Appointment newAppointment, RoomType type)
    {
        var unavailable = GetUnavailableRoomLocations(newAppointment, type);
        var available = 
            from room in _roomService.GetAll()
            where room.Type == type && !unavailable.Contains(room.Location)
            select room;
        if (!available.Any())
        {
            throw new NoAvailableRoomException("Uh-oh, no rooms available at time interval: " 
                + newAppointment.DateRange);
        }
        return available.First();  // bad way of finding, will result in some rooms getting swamped, but works
    }

    private HashSet<string> GetUnavailableRoomLocations(Appointment newAppointment, RoomType type)
    {
        HashSet<string> unavailable;
        if (type == RoomType.CHECKUP)
        {
            // TODO: this is a slow way because we convert to enumerable (?), try find a way without it
            // need convert to enumerable because complex filters
            unavailable = 
                (from appo in _appointmentRepo.GetCheckups().AsQueryable().ToEnumerable()
                where appo.DateRange.Overlaps(newAppointment.DateRange) 
                where appo.Id != newAppointment.Id
                select appo.RoomLocation).ToHashSet();
        }
        else
        {
            unavailable = 
                (from appo in _appointmentRepo.GetOperations().AsQueryable().ToEnumerable()
                where appo.DateRange.Overlaps(newAppointment.DateRange) 
                where appo.Id != newAppointment.Id
                select appo.RoomLocation).ToHashSet();
        }
        return unavailable;
    }

    public bool IsRoomAvailableForRenovation(string roomLocation, DateTime renovationStartTime)
    {
        // Does some appointment end after Renovation starts? Then can't renovate
        var checkupOvertakes = 
            (from checkup in _appointmentRepo.GetCheckups().AsQueryable()
            where renovationStartTime < checkup.DateRange.Ends
            select checkup).Any();
        if (checkupOvertakes) return false;

        var operationOvertakes = 
            (from operation in _appointmentRepo.GetOperations().AsQueryable()
            where renovationStartTime < operation.DateRange.Ends
            select operation).Any();
        return !operationOvertakes;
    }

    public List<Checkup> GetCheckupsByDoctor(ObjectId id)
    {
        var checkups = _appointmentRepo.GetCheckups();
        List<Checkup> doctorsCheckups = checkups.Find(appointment => appointment.Doctor.Id == id).ToList();
        return doctorsCheckups;
    }

    public List<Checkup> GetCheckupsByPatient(ObjectId id)
    {
        var checkups = _appointmentRepo.GetCheckups();
        List<Checkup> patientCheckups = checkups.Find(appointment => appointment.Patient.Id == id).ToList();
        return patientCheckups;
    }
    
    public List<Checkup>  SearchPastCheckups(ObjectId patientId, string anamnesisKeyword)
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

    public List<Operation> GetOperationsByPatient(ObjectId id)
    {
        var operations = _appointmentRepo.GetOperations();
        List<Operation> patientOperations = operations.Find(appointment => appointment.Patient.Id == id).ToList();
        return patientOperations;
    }

    public List<Operation> GetOperationsByDoctor(ObjectId id)
    {
        var operations = _appointmentRepo.GetOperations();
        List<Operation> doctorsOperations = operations.Find(appointment => appointment.Doctor.Id == id).ToList();
        return doctorsOperations;
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

    public List<Checkup> GetPastCheckupsByPatient(ObjectId id)
    {
        var checkups = _appointmentRepo.GetCheckups();
        List<Checkup> selectedCheckups = 
            (from checkup in checkups.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where checkup.DateRange.HasPassed() && checkup.Patient.Id == id
            select checkup).ToList();
        return selectedCheckups;
    }

    public Checkup GetCheckupById(ObjectId id)
    {
        var checkups = _appointmentRepo.GetCheckups();
        Checkup checkup = checkups.Find(appointment => appointment.Id == id).FirstOrDefault();
        return checkup;
    }
    
    public bool IsDoctorAvailable(DateRange range, Doctor doctor)
    {
        List<Checkup> checkups = GetCheckupsByDoctor(doctor.Id);
        List<Operation> operations = GetOperationsByDoctor(doctor.Id);
        foreach (Checkup checkup in checkups)
        {
            if (checkup.DateRange.Overlaps(range))
            {
                return false;
            } 
        }
        foreach (Operation operation in operations)
        {
            if (operation.DateRange.Overlaps(range))
            {
                return false;
            } 
        }
        return true;
    }

    public int CompareCheckupsByDoctorsName(Checkup checkup1, Checkup checkup2)
    {
        string name1 = _doctorRepo.GetById((ObjectId)checkup1.Doctor.Id).FirstName;
        string name2 = _doctorRepo.GetById((ObjectId)checkup2.Doctor.Id).FirstName;
        return String.Compare(name1, name2);
    }

    public int CompareCheckupsByDoctorsSpecialty(Checkup checkup1, Checkup checkup2)
    {
        string specialty1 = _doctorRepo.GetById((ObjectId)checkup1.Doctor.Id).Specialty.ToString();
        string specialty2 = _doctorRepo.GetById((ObjectId)checkup2.Doctor.Id).Specialty.ToString();
        return String.Compare(specialty1, specialty2);
    }
    
}