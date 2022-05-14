using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;


namespace HospitalSystem
{
    public class Medication
    {
        [BsonId]
        public ObjectId Id {get; set;}
        public string Name {get; set;}
        public List<string> Ingredients {get; set;}

        [BsonConstructor]
        public Medication(string name, List<string> ingredients)
        {
            Id = ObjectId.GenerateNewId();
            Name = name;
            Ingredients = ingredients;
        }
    }

    
}