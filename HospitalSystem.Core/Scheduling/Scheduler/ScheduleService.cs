using HospitalSystem.Core.Rooms;
using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core.Scheduler;

public class ScheduleService
{
    private AppointmentService _appointmentService;
    private RoomService _roomService;
    private DoctorService _doctorService;
    private PatientService _patientService;

    public ScheduleService(AppointmentService appointmentService, RoomService roomService, DoctorService doctorService, PatientService patientService)
    {
        _appointmentService =  appointmentService;
        _roomService = roomService;
        _doctorService = doctorService;
        _patientService = patientService;
    }
    
    private Room GetAvailableRoom(Appointment newAppointment, RoomType type)
    {
        var unavailable = GetUnavailableRoomLocations(newAppointment, type);
        var available = 
            from room in _roomService.GetActive()
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
                (from appo in _appointmentService.GetCheckups().AsQueryable().ToEnumerable()
                where appo.DateRange.Overlaps(newAppointment.DateRange) 
                where appo.Id != newAppointment.Id
                select appo.RoomLocation).ToHashSet();
        }
        else
        {
            unavailable = 
                (from appo in _appointmentService.GetOperations().AsQueryable().ToEnumerable()
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
            (from checkup in _appointmentService.GetCheckups().AsQueryable()
            where renovationStartTime < checkup.DateRange.Ends
            select checkup).Any();
        if (checkupOvertakes) return false;

        var operationOvertakes = 
            (from operation in _appointmentService.GetOperations().AsQueryable()
            where renovationStartTime < operation.DateRange.Ends
            select operation).Any();
        return !operationOvertakes;
    }
    
    public bool IsDoctorAvailable(DateRange range, Doctor doctor)
    {
        List<Checkup> checkups = _appointmentService.GetCheckupsByDoctor(doctor);
        List<Operation> operations = _appointmentService.GetOperationsByDoctor(doctor.Id);
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

    public void ScheduleCheckup(Checkup newCheckup)
    {
        newCheckup.RoomLocation = GetAvailableRoom(newCheckup, RoomType.CHECKUP).Location;
        _appointmentService.UpsertCheckup(newCheckup);
    }
    public void ScheduleOperation(Operation newOperation)
    {
        newOperation.RoomLocation = GetAvailableRoom(newOperation, RoomType.OPERATION).Location;
        _appointmentService.UpsertOperation(newOperation);
    }

    public bool ScheduleCheckup(User _user, DateTime dateTime, string name, string surname)
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
            ScheduleCheckup(checkup);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool ScheduleOperation(User _user, DateTime dateTime, string name, string surname, TimeSpan duration)
    {
        Patient patient = _patientService.GetPatientByFullName(name,surname);
        if (patient == null)
        {
            return false;
        }
        Doctor doctor = _doctorService.GetById((ObjectId)_user.Person.Id);
        Operation operation = new Operation(new DateRange(dateTime, dateTime.Add(duration)), new MongoDBRef("patients", patient.Id), new MongoDBRef("doctors", _user.Person.Id), "anamnesis:");
        if (IsDoctorAvailable(operation.DateRange, doctor))
        {
            ScheduleOperation(operation);
            return true;
        }
        else
        {
            return false;
        }
    }

    //TO-DO: refactor this method
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

    //TO-DO: refactor this method
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

    //TO-DO: refactor this method
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
        
        return GetFirstFewFreeCheckups(doctor,3,user);
    }
    
}