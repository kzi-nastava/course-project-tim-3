using HospitalSystem.Core.Rooms;

namespace HospitalSystem.Core.Equipment;

[System.Serializable]
public class EquipmentBatchDoesNotExistException : System.Exception
{
    public EquipmentBatchDoesNotExistException() { }
    public EquipmentBatchDoesNotExistException(string message) : base(message) { }
    public EquipmentBatchDoesNotExistException(string message, System.Exception inner) : base(message, inner) { }
    protected EquipmentBatchDoesNotExistException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class EquipmentBatchService
{
    private IEquipmentBatchRepository _repo;

    public EquipmentBatchService(IEquipmentBatchRepository repo)
    {
        _repo = repo;
    }

    public IQueryable<EquipmentBatch> GetAllExisting()
    {
        return _repo.GetAllExisting();
    }

    public IQueryable<EquipmentBatch> GetAllIn(string roomLocation)
    {
        return _repo.GetAllIn(roomLocation);
    }

    public void Add(EquipmentBatch newBatch)
    {
        var batch = _repo.Get(newBatch.RoomLocation, newBatch.Name);
        if (batch is null)
        {
            _repo.Insert(newBatch);
        }
        else
        {
            batch.MergeWith(newBatch);
            _repo.Replace(batch);
        }
    }

    // NOTE: only use during Relocation!!
    public void Remove(EquipmentBatch removingBatch)
    {
        var existingBatch = _repo.Get(removingBatch.RoomLocation, removingBatch.Name);
        if (existingBatch is null)
        {
            throw new EquipmentBatchDoesNotExistException("That batch does not exist. How did you get here?");
        }
        else
        {
            existingBatch.Remove(removingBatch);
            if (existingBatch.Count != 0)
                _repo.Replace(existingBatch);
            else
                _repo.Delete(existingBatch);
        }
    }

    public void DeleteAllInRoom(Room room)
    {
        _repo.DeleteMany(batch => batch.RoomLocation == room.Location);
    }

    public IQueryable<EquipmentBatch> SearchExisting(EquipmentQuery query)
    {
        return _repo.SearchExisting(query);
    }

    public List<EquipmentBatch> GetLow()
    {
        List<EquipmentBatch> equipments = _repo.GetAll().ToList();
        equipments.RemoveAll(eq => eq.Count > 5);
        equipments.Sort(delegate(EquipmentBatch x, EquipmentBatch y)
        {
            return x.Count.CompareTo(y.Count);
        });
        return equipments;
    }

    public  List<EquipmentBatch> GetExistingByName(string name)
    {
        List<EquipmentBatch> equipments = _repo.GetAll().ToList();
        equipments.RemoveAll(eq => eq.Name != name);
        equipments.RemoveAll(eq => eq.Count == 0);
        equipments.Sort(delegate(EquipmentBatch left, EquipmentBatch right){
            return left.Count.CompareTo(right.Count);
        });
        return equipments;
    }
    
    public List<EquipmentAmount> GetEmpty()
    {
        return _repo.GetMissing();
    }
}