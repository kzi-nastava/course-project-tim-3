using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem
{
    [BsonKnownTypes(typeof(Patient), typeof(Doctor), typeof(Secretary), typeof(Director))]
    public abstract class Person
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public Person(string firstName, string lastName) {
            Id = ObjectId.GenerateNewId();
            FirstName = firstName;
            LastName = lastName;
        }
    }
} 
