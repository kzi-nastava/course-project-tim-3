using System.Linq.Expressions;

namespace HospitalSystem.Core;

public interface IEquipmentBatchRepository
{
    public IQueryable<EquipmentBatch> GetAll();

    public EquipmentBatch? Get(string roomLocation, string name);

    public void Insert(EquipmentBatch batch);

    public void Replace(EquipmentBatch batch); // EXPECTS EXISTING EQUIPMENTBATCH!

    public void Delete(EquipmentBatch batch);

    public void DeleteMany(Expression<Func<EquipmentBatch, bool>> filter);
}
