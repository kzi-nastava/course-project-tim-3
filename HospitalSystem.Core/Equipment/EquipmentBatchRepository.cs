using MongoDB.Driver;
using System.Linq.Expressions;

namespace HospitalSystem.Core;

public class EquipmentBatchRepository : IEquipmentBatchRepository
{
    private MongoClient _dbClient;

    public EquipmentBatchRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<EquipmentBatch> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<EquipmentBatch>("equipment");
    }

    public IQueryable<EquipmentBatch> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public EquipmentBatch? Get(string roomLocation, string name)
    {
        var batches = GetMongoCollection();
        return batches.Find(batch => batch.RoomLocation == roomLocation && batch.Name == name).FirstOrDefault();
    }

    public void Insert(EquipmentBatch batch)
    {
        GetMongoCollection().InsertOne(batch);
    }

    public void Replace(EquipmentBatch newBatch)
    {
        GetMongoCollection().ReplaceOne(batch => batch.Id == newBatch.Id, newBatch);
    }

    public void Delete(EquipmentBatch existingBatch)
    {
        GetMongoCollection().DeleteOne(batch => batch.Id == existingBatch.Id);
    }

    public void DeleteMany(Expression<Func<EquipmentBatch, bool>> filter)
    {
        GetMongoCollection().DeleteMany(filter);
    }

    public IQueryable<EquipmentBatch> Search(EquipmentQuery query)
    {
        return
            from batch in GetAll()
            where (query.MinCount == null || query.MinCount <= batch.Count)
                && (query.MaxCount == null || query.MaxCount >= batch.Count)
                && (query.Type == null || query.Type == batch.Type)
                && (query.NameContains == null || query.NameContains.IsMatch(batch.Name))
            select batch;
    }

    public IQueryable<EquipmentBatch> GetAllIn(string roomLocation)
    {
        return
            from batch in GetAll()
            where batch.RoomLocation == roomLocation
            select batch;
    }

    //TODO: Find a better name, GetMissing is taken...
    public List<EquipmentAmount> GetEmpty()
    {
        return GetMongoCollection().Aggregate().Group(equipment => equipment.Name, 
            group => new EquipmentAmount(group.Key, group.Sum(equipment => equipment.Count))).ToList();
    }
}