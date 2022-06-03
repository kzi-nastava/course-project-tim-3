using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

public class EquipmentRelocationService
{
    private IEquipmentRelocationRepository _relocationRepo;
    private EquipmentBatchService _equipmentService;

    public EquipmentRelocationService(IEquipmentRelocationRepository relocationRepo,
        EquipmentBatchService equipmentService)
    {
        _relocationRepo = relocationRepo;
        _equipmentService = equipmentService;
    }

    private void MoveEquipment(EquipmentRelocation relocation)
    {
        var removing = new EquipmentBatch(relocation.FromRoomLocation, relocation.Name,
            relocation.Count, relocation.Type);
        var adding = new EquipmentBatch(relocation.ToRoomLocation, relocation.Name,
            relocation.Count, relocation.Type);
        _equipmentService.Remove(removing);
        _equipmentService.Add(adding);
        relocation.IsDone = true;
        _relocationRepo.Replace(relocation);
    }

    public void MoveAll(string fromLocation, string toLocation)
    {
        foreach (var batch in _equipmentService.GetAllIn(fromLocation))
        {
            _equipmentService.Remove(batch);
            batch.RoomLocation = toLocation;
            _equipmentService.Add(batch);
        }
    }

    public void Schedule(EquipmentRelocation relocation)
    {
        _relocationRepo.Insert(relocation);
        RegisterFinishCallback(relocation);
    }

    private void RegisterFinishCallback(EquipmentRelocation relocation)
    {
        CallbackScheduler.Register(relocation.EndTime, () => 
        {
            MoveEquipment(relocation);
        });
    }

    public void ScheduleAll()
    {
        foreach (var relocation in _relocationRepo.GetAll())
        {
            if (!relocation.IsDone)
            {
                RegisterFinishCallback(relocation);
            }
        }
    }
}