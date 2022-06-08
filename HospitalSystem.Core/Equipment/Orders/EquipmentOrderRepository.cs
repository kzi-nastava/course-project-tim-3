using MongoDB.Driver;

namespace HospitalSystem.Core;

public class EquipmentOrderRepository : IEquipmentOrderRepository
{
    private MongoClient _dbClient;

    public EquipmentOrderRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<EquipmentOrder> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<EquipmentOrder>("equipment_orders");
    }

    public IQueryable<EquipmentOrder> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Insert(EquipmentOrder relocation)
    {
        GetMongoCollection().InsertOne(relocation);
    }

    public void Replace(EquipmentOrder replacing)
    {
        GetMongoCollection().ReplaceOne(relocation => relocation.Id == replacing.Id, replacing);
    }
}