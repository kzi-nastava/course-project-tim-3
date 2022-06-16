using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem.Core;

[BsonKnownTypes(typeof(Patient), typeof(Doctor))]
public class Person
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