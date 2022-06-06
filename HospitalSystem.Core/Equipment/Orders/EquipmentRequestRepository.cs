using MongoDB.Driver;

namespace HospitalSystem.Core;

public class EquipmentRequestRepository : IEquipmentRequestRepository
{
    private MongoClient _dbClient;

    public EquipmentRequestRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<EquipmentRequest> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<EquipmentRequest>("equipment_requests");
    }

    public IQueryable<EquipmentRequest> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Insert(EquipmentRequest relocation)
    {
        GetMongoCollection().InsertOne(relocation);
    }

    public void Replace(EquipmentRequest replacing)
    {
        GetMongoCollection().ReplaceOne(relocation => relocation.Id == replacing.Id, replacing);
    }
}