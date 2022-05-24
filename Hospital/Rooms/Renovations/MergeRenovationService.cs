using HospitalSystem.Utils;

namespace HospitalSystem;

public class MergeRenovationService
{
    private IMergeRenovationRepository _repo;
    private RoomService _roomService;
    private EquipmentRelocationService _relocationService;

    public MergeRenovationService(IMergeRenovationRepository repo, RoomService roomService,
        EquipmentRelocationService relocationService)
    {
        _repo = repo;
        _roomService = roomService;
        _relocationService = relocationService;
    }

    public void Schedule(MergeRenovation renovation, Room mergingInto)
    {
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