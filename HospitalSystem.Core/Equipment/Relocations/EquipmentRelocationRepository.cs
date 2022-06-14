using MongoDB.Driver;

namespace HospitalSystem.Core.Equipment.Relocations;

public class EquipmentRelocationRepository : IEquipmentRelocationRepository
{
    private MongoClient _dbClient;

    public EquipmentRelocationRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<EquipmentRelocation> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<EquipmentRelocation>("relocations");
    }

    public IQueryable<EquipmentRelocation> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Insert(EquipmentRelocation relocation)
    {
        GetMongoCollection().InsertOne(relocation);
    }

    public void Replace(EquipmentRelocation replacing)
    {
        GetMongoCollection().ReplaceOne(relocation => relocation.Id == replacing.Id, replacing);
    }
}