using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace Hospital;

public class EquipmentRepository
{
    private MongoClient _dbClient;

    public EquipmentRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }

    public IMongoCollection<Equipment> GetEquipments()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Equipment>("equipments");
    }

    public IMongoQueryable<Equipment> GetQueryableEquipments()
    {
        return GetEquipments().AsQueryable();
    }

    public void AddEquipment(Equipment newEquipment)
    {
        var equipment = GetEquipment((ObjectId) newEquipment.Room.Id, newEquipment.Name);
        if (equipment is null)
        {
            GetEquipments().InsertOne(newEquipment);
        }
        else
        {
            equipment.MergeWith(newEquipment);
            UpdateEquipment(equipment);
        }
    }

    // TODO: remove Equipment when deleting room?

    private void UpdateEquipment(Equipment newEquipment) // EXPECTS EXISTING EQUIPMENT!
    {
        var equipments = GetEquipments();
        equipments.ReplaceOne(equipment => equipment.Id == newEquipment.Id, newEquipment);
    }

    public Equipment? GetEquipment(ObjectId roomId, string name)
    {
        var equipments = GetEquipments();
        var existingEquipment = equipments.Find(equipment => equipment.Room.Id == roomId 
                                                                && equipment.Name == name);
        return existingEquipment.FirstOrDefault();
    }
}