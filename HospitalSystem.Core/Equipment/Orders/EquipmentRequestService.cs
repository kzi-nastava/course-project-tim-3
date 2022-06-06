using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

public class EquipmentRequestService
{
    private IEquipmentRequestRepository _requestRepo;
    private EquipmentBatchService _equipmentService;

    public EquipmentRequestService(IEquipmentRequestRepository requestRepo,
        EquipmentBatchService equipmentService)
    {
        _requestRepo = requestRepo;
        _equipmentService = equipmentService;
    }

    private void MoveEquipment(EquipmentRequest request)
    {
        var adding = new EquipmentBatch(request.ToStockLocation, request.Name,
            request.Count, request.Type);
        _equipmentService.Add(adding);
        request.IsDone = true;
        _requestRepo.Replace(request);
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

    public void Schedule(EquipmentRequest request)
    {
        _requestRepo.Insert(request);
        RegisterFinishCallback(request);
    }

    private void RegisterFinishCallback(EquipmentRequest request)
    {
        CallbackScheduler.Register(request.EndTime, () => 
        {
            MoveEquipment(request);
        });
    }

    public void ScheduleAll()
    {
        foreach (var request in _requestRepo.GetAll())
        {
            if (!request.IsDone)
            {
                RegisterFinishCallback(request);
            }
        }
    }
}