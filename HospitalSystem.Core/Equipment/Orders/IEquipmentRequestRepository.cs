namespace HospitalSystem.Core;

public interface IEquipmentRequestRepository
{
    public IQueryable<EquipmentRequest> GetAll();
    
    public void Insert(EquipmentRequest relocation);

    public void Replace(EquipmentRequest relocation); 
}