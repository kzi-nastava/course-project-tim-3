using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Hospital
{
    public enum Role
    {
        DOCTOR,
        PATIENT,
        SECRETARY,
        DIRECTOR
    }

    [BsonIgnoreExtraElements]
    public class User
    {
        // todo: might want to add objectId here
        public string Username {get;}
        public string Password {get;}
        [BsonRepresentation(BsonType.String)]
        public Role Role {get;}
        public Person Person {get;}

        public User(string username, string password, string firstName, string lastName, Role role)
        {
            this.Username = username;
            this.Password = password;
            this.Role = role;
            // TODO: everyone is patient. add a switch case here
            Person = new Patient(firstName, lastName);
        }

        [BsonConstructor]
        public User(string username, string password, Person person, Role role)
        {
            this.Username = username;
            this.Password = password;
            this.Role = role;
            this.Person = person;
        }
    }
} 
