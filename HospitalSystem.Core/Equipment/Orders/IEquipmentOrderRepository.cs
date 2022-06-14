namespace HospitalSystem.Core.Equipment;

public interface IEquipmentOrderRepository
{
    public IQueryable<EquipmentOrder> GetAll();
    
    public void Insert(EquipmentOrder eqRequest);

    public void Replace(EquipmentOrder eqRequest); 
}