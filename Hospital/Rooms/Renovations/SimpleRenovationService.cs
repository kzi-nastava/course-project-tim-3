using HospitalSystem.Utils;

namespace HospitalSystem;

public class SimpleRenovationService
{
    private ISimpleRenovationRepository _simpleRenovationRepo;
    private RoomService _roomService;

    public SimpleRenovationService(ISimpleRenovationRepository simpleRenovationRepo, RoomService roomService)
    {
        _simpleRenovationRepo = simpleRenovationRepo;
        _roomService = roomService;
    }

    public void Schedule(SimpleRenovation renovation)
    {
        _simpleRenovationRepo.Insert(renovation);

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
        _simpleRenovationRepo.Replace(renovation);
        _roomService.Activate(renovation.RoomLocation);
    }

    public void ScheduleAll()
    {
        foreach (var renovation in _simpleRenovationRepo.GetAll())
        {
            if (!renovation.IsDone)
            {
                Schedule(renovation);
            }
        }
    }
}