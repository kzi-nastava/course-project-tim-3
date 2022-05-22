namespace HospitalSystem;

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

    public IQueryable<EquipmentBatch> GetAll()
    {
        return _repo.GetAll();
    }

    public IQueryable<EquipmentBatch> GetAllIn(string roomLocation)
    {
        var batches = _repo.GetAll();
        var matches = 
            from batch in batches
            where batch.RoomLocation == roomLocation
            select batch;
        return matches;
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

    public IQueryable<EquipmentBatch> Search(EquipmentQuery query)
    {
        var batches = _repo.GetAll();
        var matches = 
            from batch in batches
            where (query.MinCount == null || query.MinCount <= batch.Count)
                && (query.MaxCount == null || query.MaxCount >= batch.Count)
                && (query.Type == null || query.Type == batch.Type)
                && (query.NameContains == null || query.NameContains.IsMatch(batch.Name))
            select batch;
        return matches;
    }
}