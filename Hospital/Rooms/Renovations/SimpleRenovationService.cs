using HospitalSystem.Utils;

namespace HospitalSystem;

[System.Serializable]
public class RenovationException : System.Exception
{
    public RenovationException() { }
    public RenovationException(string message) : base(message) { }
    public RenovationException(string message, System.Exception inner) : base(message, inner) { }
    protected RenovationException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class SimpleRenovationService
{
    private ISimpleRenovationRepository _repo;
    private RoomService _roomService;
    private AppointmentRepository _appointmentRepo;

    public SimpleRenovationService(ISimpleRenovationRepository repo, RoomService roomService,
        AppointmentRepository appointmentRepo)
    {
        _repo = repo;
        _roomService = roomService;
        _appointmentRepo = appointmentRepo;
    }

    public void Schedule(SimpleRenovation renovation)
    {
        if (!_appointmentRepo.IsRoomAvailableForRenovation(renovation.RoomLocation, renovation.BusyRange.Starts))
        {
            throw new RenovationException("That room has appointments scheduled, can't renovate");
        }
        _repo.Insert(renovation);
        JustSchedule(renovation);
    }

    private void JustSchedule(SimpleRenovation renovation)
    {
        Scheduler.Schedule(renovation.BusyRange.Starts, () =>
        {
            _roomService.Deactivate(renovation.RoomLocation);
        });
        Scheduler.Schedule(renovation.BusyRange.Ends, () => 
        {
            FinishRenovation(renovation);
        });
    }

    private void FinishRenovation(SimpleRenovation renovation)
    {
        renovation.IsDone = true;
        _repo.Replace(renovation);
        _roomService.Activate(renovation.RoomLocation);
    }

    public void ScheduleAll()
    {
        foreach (var renovation in _repo.GetAll())
        {
            if (!renovation.IsDone)
            {
                JustSchedule(renovation);
            }
        }
    }
}