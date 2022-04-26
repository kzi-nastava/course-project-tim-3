using MongoDB.Bson.Serialization.Attributes;

namespace Hospital
{
    [BsonKnownTypes(typeof(Patient))]
    public abstract class Person
    {
        public string FirstName {get;}
        public string LastName {get;}

        public Person(string firstName, string lastName) {
            FirstName = firstName;
            LastName = lastName;
        }
    }
} 
