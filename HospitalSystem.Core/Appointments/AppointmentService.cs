using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;

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
        newCheckup.RoomLocation = GetAvailableRoom(newCheckup, RoomType.CHECKUP).Location;
        _appointmentRepo.UpsertCheckup(newCheckup);
    }

    public bool UpsertCheckup(User _user, DateTime dateTime, string name, string surname)
    {
        Patient patient = _patientService.GetPatientByFullName(name,surname);
        if (patient == null)
        {
            return false;
        }
        Doctor doctor = _doctorService.GetById((ObjectId)_user.Person.Id);
        Checkup checkup = new Checkup(dateTime, new MongoDBRef("patients", patient.Id), new MongoDBRef("doctors", _user.Person.Id), "anamnesis:");
        if (IsDoctorAvailable(checkup.DateRange, doctor))
        {
            _appointmentRepo.UpsertCheckup(checkup);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpsertOperation(Operation newOperation)
    {
        newOperation.RoomLocation = GetAvailableRoom(newOperation, RoomType.OPERATION).Location;
        _appointmentRepo.UpsertOperation(newOperation);
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
            (from checkup in _appointmentRepo.GetCheckups().AsQueryable().ToList()
            where renovationStartTime < checkup.DateRange.Ends
            select checkup).Any();
        if (checkupOvertakes) return false;

        var operationOvertakes = 
            (from operation in _appointmentRepo.GetOperations().AsQueryable().ToList()
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

    public List<Operation> GetPastOperationsByPatient(ObjectId id)
    {
        var operations = _appointmentRepo.GetOperations();
        List<Operation> patientOperations = 
            (from operation in operations.AsQueryable().ToList()  // TODO: inefficient, but bug fix
            where operation.DateRange.HasPassed() && operation.Patient.Id == id
            select operation).ToList();
        return patientOperations;
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
        string name1 = _doctorService.GetById((ObjectId)checkup1.Doctor.Id).FirstName;
        string name2 = _doctorService.GetById((ObjectId)checkup2.Doctor.Id).FirstName;
        return String.Compare(name1, name2);
    }

    public int CompareCheckupsByDoctorsSpecialty(Checkup checkup1, Checkup checkup2)
    {
        string specialty1 = _doctorService.GetById((ObjectId)checkup1.Doctor.Id).Specialty.ToString();
        string specialty2 = _doctorService.GetById((ObjectId)checkup2.Doctor.Id).Specialty.ToString();
        return String.Compare(specialty1, specialty2);
    }

    public List<Checkup> GetEarliestFreeCheckups(DateRange interval, Specialty speciality, int numberOfCheckups, User user)
    {
        List<Checkup> checkups = new List<Checkup>();
        DateTime iterationDate = RoundUp(DateTime.Now,TimeSpan.FromMinutes(15));

        while ( checkups.Count < numberOfCheckups)
        { 
            if (iterationDate.TimeOfDay >=Globals.ClosingTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day, Globals.OpeningTime.Hour, Globals.OpeningTime.Minute, Globals.OpeningTime.Second);
                iterationDate = iterationDate.AddDays(1);
                continue;
            }

            if (iterationDate.TimeOfDay < Globals.OpeningTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day, Globals.OpeningTime.Hour, Globals.OpeningTime.Minute, Globals.OpeningTime.Second);
                continue;
            }

            if (!(interval.Starts.TimeOfDay<=iterationDate.TimeOfDay && iterationDate.TimeOfDay<interval.Ends.TimeOfDay))
            {
                iterationDate = iterationDate.AddMinutes(15);
                continue;
            }

            foreach (Doctor doctor in _doctorService.GetManyBySpecialty(speciality))
            {
                Checkup newCheckup = new Checkup(
                    iterationDate,
                    new MongoDB.Driver.MongoDBRef("patients", user.Person.Id),
                    new MongoDB.Driver.MongoDBRef("doctors", doctor.Id),
                    "no anamnesis");
                if (!IsDoctorAvailable(newCheckup.DateRange, doctor))
                {
                    continue;
                }
                else
                {
                    if (checkups.Count >= numberOfCheckups)
                    {
                        break;
                    }

                    checkups.Add(newCheckup);
                }
            }
            iterationDate = iterationDate.AddMinutes(15);
        }
        return checkups;
    }

    public List<Checkup> GetFirstFewFreeCheckups(Doctor doctor, int numberOfCheckups, User user)
    {
        List<Checkup> checkups = new List<Checkup>();
        DateTime iterationDate = RoundUp(DateTime.Now, TimeSpan.FromMinutes(15));

        while (checkups.Count < numberOfCheckups)
        {
            if (iterationDate.TimeOfDay >=Globals.ClosingTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day,
                    Globals.OpeningTime.Hour, Globals.OpeningTime.Minute, Globals.OpeningTime.Second);
                iterationDate = iterationDate.AddDays(1);
                continue;
            }

            if (iterationDate.TimeOfDay < Globals.OpeningTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day,
                    Globals.OpeningTime.Hour, Globals.OpeningTime.Minute, Globals.OpeningTime.Second);
                continue;
            }
            
            Checkup newCheckup = new Checkup(
                iterationDate,
                new MongoDB.Driver.MongoDBRef("patients", user.Person.Id),
                new MongoDB.Driver.MongoDBRef("doctors", doctor.Id),
                "no anamnesis");
            if (!IsDoctorAvailable(newCheckup.DateRange, doctor))
            {
                iterationDate = iterationDate.AddMinutes(15);
                continue;
            }
            else
            {
                checkups.Add(newCheckup);
                iterationDate = iterationDate.AddMinutes(15);
            }
        }
        return checkups;
    }

    public DateTime RoundUp(DateTime dt, TimeSpan d)
    {
        return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
    }

    public List<Checkup> FindSuitableCheckups(Doctor doctor, DateRange interval, DateTime deadline, bool isIntervalPriority, User user)
    {
        List<Checkup> checkups = new List<Checkup>();
        DateTime iterationDate = RoundUp(DateTime.Now,TimeSpan.FromMinutes(15));
        while ( iterationDate < deadline)
        {
            if (iterationDate.TimeOfDay >=Globals.ClosingTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day, Globals.OpeningTime.Hour, Globals.OpeningTime.Minute, Globals.OpeningTime.Second);
                iterationDate = iterationDate.AddDays(1);
                continue;
            }

            if (iterationDate.TimeOfDay < Globals.OpeningTime.TimeOfDay)
            {
                iterationDate = new DateTime(iterationDate.Year, iterationDate.Month, iterationDate.Day, Globals.OpeningTime.Hour, Globals.OpeningTime.Minute, Globals.OpeningTime.Second);
                continue;
            }

            if (!(interval.Starts.TimeOfDay<=iterationDate.TimeOfDay && iterationDate.TimeOfDay<interval.Ends.TimeOfDay))
            {
                iterationDate = iterationDate.AddMinutes(15);
                continue;
            }
            
            Checkup newCheckup = new Checkup(
                iterationDate,
                new MongoDB.Driver.MongoDBRef("patients", user.Person.Id),
                new MongoDB.Driver.MongoDBRef("doctors", doctor.Id),
                "no anamnesis");
            if (!IsDoctorAvailable(newCheckup.DateRange, doctor))
            {
                iterationDate = iterationDate.AddMinutes(15);
                continue;
            }
            else
            {
                checkups.Add(newCheckup);
                return checkups;
            }
        }
        //if code gets to this point, it means it hasnt found a good match
        if (isIntervalPriority)
        {
            return GetEarliestFreeCheckups(interval,doctor.Specialty,3, user);
        }
        return GetFirstFewFreeCheckups(doctor,3,user);
    }

    public List<Checkup> FindCheckupsPriorityInterval(Doctor doctor, DateTime intervalStart, DateTime intervalEnd, DateTime deadline)
    {
        List<Checkup> checkups = new List<Checkup>();
        return checkups;
    }
    
}