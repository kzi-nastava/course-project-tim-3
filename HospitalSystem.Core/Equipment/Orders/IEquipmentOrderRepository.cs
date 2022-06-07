namespace HospitalSystem.Core;

public interface IEquipmentOrderRepository
{
    public IQueryable<EquipmentOrder> GetAll();
    
    public void Insert(EquipmentOrder eqRequest);

    public void Replace(EquipmentOrder eqRequest); 
}