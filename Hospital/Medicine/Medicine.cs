using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;


namespace Hospital
{
    public class Medicine
    {
        [BsonId]
        public ObjectId Id {get; set;}
        public string Name {get; set;}
        public List<string> Ingredients {get; set;}

        [BsonConstructor]
        public Medicine(string name, List<string> ingredients)
        {
            Id = ObjectId.GenerateNewId();
            Name = name;
            Ingredients = ingredients;
        }
    }

    
}