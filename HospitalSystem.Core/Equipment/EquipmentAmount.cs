using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalSystem.Core;

public class EquipmentAmount
{
    public string Name {get; set;}

    public int Amount{get; set;}

    public EquipmentAmount(string name, int amount)
    {
        Name = name;
        Amount = amount;
    }
}