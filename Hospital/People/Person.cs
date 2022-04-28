using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Hospital
{
    [BsonKnownTypes(typeof(Patient), typeof(Doctor))]
    
    public abstract class Person
    {
        [BsonId]
        public ObjectId Id { get; set; }
        private int myVar;
        public string FirstName {get; set;}
        public string LastName {get; set;}

        public Person(string firstName, string lastName) {
            Id = ObjectId.GenerateNewId();
            FirstName = firstName;
            LastName = lastName;
        }
    }
} 
