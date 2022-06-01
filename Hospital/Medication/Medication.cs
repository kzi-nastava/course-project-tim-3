using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalSystem;

public class Medication
{
    [BsonId]
    public ObjectId Id { get; }
    public string Name { get; set; }
    [BsonElement]
    public List<string> Ingredients { get; }

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

    public void RemoveIngredient(string ingredient)
    {
        Ingredients.Remove(ingredient);
    }

    public void AddIngredient(string ingredient) // TODO: use these instead
    {
        Ingredients.Add(ingredient);
    }
}