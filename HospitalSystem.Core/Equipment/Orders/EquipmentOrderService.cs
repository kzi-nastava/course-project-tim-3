using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core.Equipment;

public class EquipmentOrderService
{
    private IEquipmentOrderRepository _orderRepo;
    private EquipmentBatchService _equipmentService;

    public EquipmentOrderService(IEquipmentOrderRepository orderRepo, EquipmentBatchService equipmentService)
    {
        _orderRepo = orderRepo;
        _equipmentService = equipmentService;
    }

    private void AddEquipment(EquipmentOrder order)
    {
        var adding = new EquipmentBatch(order.ToStockLocation, order.Name, order.Count, order.Type);
        _equipmentService.Insert(adding);
        order.IsDone = true;
        _orderRepo.Replace(order);
    }

    public void Schedule(EquipmentOrder order)
    {
        _orderRepo.Insert(order);
        RegisterFinishCallback(order);
    }

    private void RegisterFinishCallback(EquipmentOrder order)
    {
        CallbackScheduler.Register(order.EndTime, () => 
        {
            AddEquipment(order);
        });
    }

    public void ScheduleAll()
    {
        foreach (var order in _orderRepo.GetAll())
        {
            if (!order.IsDone)
            {
                RegisterFinishCallback(order);
            }
        }
    }
}