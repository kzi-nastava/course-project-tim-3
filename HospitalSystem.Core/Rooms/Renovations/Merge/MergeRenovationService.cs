using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

public class MergeRenovationService
{
    private IMergeRenovationRepository _repo;
    private RoomService _roomService;
    private EquipmentRelocationService _relocationService;
    private AppointmentService _appointmentService;

    public MergeRenovationService(IMergeRenovationRepository repo, RoomService roomService,
        EquipmentRelocationService relocationService, AppointmentService appointmentService)
    {
        _repo = repo;
        _roomService = roomService;
        _relocationService = relocationService;
        _appointmentService = appointmentService;
    }

    public void Schedule(MergeRenovation renovation, Room mergingInto)
    {
        if (!_appointmentService.IsRoomAvailableForRenovation(renovation.FirstLocation, renovation.BusyRange.Starts))
        {  
            throw new RenovationException("First room has appointments scheduled, can't renovate.");
        }
        if (!_appointmentService.IsRoomAvailableForRenovation(renovation.SecondLocation, renovation.BusyRange.Starts))
        {  
            throw new RenovationException("Second room has appointments scheduled, can't renovate.");
        }
        _roomService.UpsertInactive(mergingInto);
        _repo.Insert(renovation);
        JustSchedule(renovation);
    }

    private void JustSchedule(MergeRenovation renovation)
    {
        Scheduler.Schedule(renovation.BusyRange.Starts, () =>
        {
            _roomService.Deactivate(renovation.FirstLocation);
            _roomService.Deactivate(renovation.SecondLocation);
        });
        Scheduler.Schedule(renovation.BusyRange.Ends, () => 
        {
            FinishRenovation(renovation);
        });
    }

    private void FinishRenovation(MergeRenovation renovation)
    {
        _roomService.Activate(renovation.MergeToLocation);
        _relocationService.MoveAll(renovation.FirstLocation, renovation.MergeToLocation);
        _relocationService.MoveAll(renovation.SecondLocation, renovation.MergeToLocation);
        _roomService.Delete(renovation.FirstLocation);
        _roomService.Delete(renovation.SecondLocation);
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