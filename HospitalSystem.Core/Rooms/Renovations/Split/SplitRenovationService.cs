using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

public class SplitRenovationService
{
    private ISplitRenovationRepository _repo;
    private RoomService _roomService;
    private EquipmentRelocationService _relocationService;
    private AppointmentService _appointmentService;

    public SplitRenovationService(ISplitRenovationRepository repo, RoomService roomService,
        EquipmentRelocationService relocationService, AppointmentService appointmentService)
    {
        _repo = repo;
        _roomService = roomService;
        _relocationService = relocationService;
        _appointmentService = appointmentService;
    }

    public void Schedule(SplitRenovation renovation, Room firstSplit, Room secondSplit)
    {
        if (!_appointmentService.IsRoomAvailableForRenovation(renovation.SplitRoomLocation, renovation.BusyRange.Starts))
        {
            throw new RenovationException("That room has appointments scheduled, can't renovate");
        }
        _roomService.UpsertInactive(firstSplit);
        _roomService.UpsertInactive(secondSplit);
        _repo.Insert(renovation);
        JustSchedule(renovation);
    }

    private void JustSchedule(SplitRenovation renovation)  // TODO: think of a better name
    {
        Scheduler.Schedule(renovation.BusyRange.Starts, () =>
        {
            _roomService.Deactivate(renovation.SplitRoomLocation);
        });
        Scheduler.Schedule(renovation.BusyRange.Ends, () => 
        {
            FinishRenovation(renovation);
        });
    }

    private void FinishRenovation(SplitRenovation renovation)
    {
        _roomService.Activate(renovation.SplitToFirstLocation);
        _roomService.Activate(renovation.SplitToSecondLocation);
        _relocationService.MoveAll(renovation.SplitRoomLocation, renovation.SplitToFirstLocation);
        _roomService.Delete(renovation.SplitRoomLocation);
        renovation.IsDone = true;
        _repo.Replace(renovation);
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