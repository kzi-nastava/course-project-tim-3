using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalSystem.Core;

public class Medication
{
    [BsonId]
    public ObjectId Id { get; }
    public string Name { get; set; }
    [BsonElement]
    public List<string> Ingredients { get; }  // TODO: add add and remove func instead of get this and adding...

    public Medication(string name, List<string> ingredients)
    {
        Id = ObjectId.GenerateNewId();
        Name = name;
        Ingredients = ingredients;
    }

    [BsonConstructor]
    internal Medication(ObjectId id, string name, List<string> ingredients)
    {
        Id = id;
        Name = name;
        Ingredients = ingredients;
    }

    public override string ToString()
    {
        return  Name;
    }
}