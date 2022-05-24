using HospitalSystem.Utils;

namespace HospitalSystem;

public class SplitRenovationService
{
    private ISplitRenovationRepository _repo;
    private RoomService _roomService;
    private EquipmentRelocationService _relocationService;

    public SplitRenovationService(ISplitRenovationRepository repo, RoomService roomService,
        EquipmentRelocationService relocationService)
    {
        _repo = repo;
        _roomService = roomService;
        _relocationService = relocationService;
    }

    public void Schedule(SplitRenovation renovation, Room firstSplit, Room secondSplit)
    {
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