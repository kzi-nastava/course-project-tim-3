using HospitalSystem.Utils;

namespace HospitalSystem;

public class SimpleRenovationService
{
    private ISimpleRenovationRepository _repo;
    private RoomService _roomService;

    public SimpleRenovationService(ISimpleRenovationRepository repo, RoomService roomService)
    {
        _repo = repo;
        _roomService = roomService;
    }

    public void Schedule(SimpleRenovation renovation)
    {
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