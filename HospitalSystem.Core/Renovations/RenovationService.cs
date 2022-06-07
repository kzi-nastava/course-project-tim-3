using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

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

public class RenovationService
{
    private IRenovationRepository _repo;
    private RoomService _roomService;
    private EquipmentRelocationService _relocationService;
    private AppointmentService _appointmentService;

    public RenovationService(IRenovationRepository repo, RoomService roomService,
        EquipmentRelocationService relocationService, AppointmentService appointmentService)
    {
        _repo = repo;
        _roomService = roomService;
        _relocationService = relocationService;
        _appointmentService = appointmentService;
    }

    public void Schedule(Renovation renovation, IList<Room> newRooms)
    {
        foreach (var loc in renovation.OldLocations)
        {
            if (!_appointmentService.IsRoomAvailableForRenovation(loc, renovation.BusyRange.Starts))
            {  
                throw new RenovationException("Room " + loc + " has appointments scheduled, can't renovate.");
            }
        }
        foreach (var room in newRooms)
        {
            _roomService.UpsertInactive(room);
        }
        _repo.Insert(renovation);
        RegisterStartCallback(renovation);
        RegisterFinishCallback(renovation);
    }

    private void RegisterStartCallback(Renovation renovation)
    {
        CallbackScheduler.Register(renovation.BusyRange.Starts, () =>
        {
            StartRenovation(renovation);
        });
    }

    private void RegisterFinishCallback(Renovation renovation)
    {
        CallbackScheduler.Register(renovation.BusyRange.Ends, () => 
        {
            FinishRenovation(renovation);
        });
    }

    private void StartRenovation(Renovation renovation)
    {
        foreach (var loc in renovation.OldLocations)
        {
            _roomService.Deactivate(loc);
        }
    }

    private void FinishRenovation(Renovation renovation)
    {
        foreach (var loc in renovation.NewLocations)
        {
            _roomService.Activate(loc);
        }
        foreach (var loc in renovation.OldLocations.Except(renovation.NewLocations))
        {
            _relocationService.MoveAll(loc, renovation.NewLocations[0]);
            _roomService.Delete(loc);
        }
        renovation.IsDone = true;
        _repo.Replace(renovation);
    }

    public void ScheduleAll()
    {
        foreach (var renovation in _repo.GetAll())
        {
            if (!renovation.IsDone)
            {
                RegisterStartCallback(renovation);
                RegisterFinishCallback(renovation);
            }
        }
    }
}