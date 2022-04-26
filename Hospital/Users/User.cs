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
        public string username {get;}
        public string password {get;}
        public Role role {get;}
        public Person person {get;}

        public User(string username, string password, string firstName, string lastName, Role role)
        {
            this.username = username;
            this.password = password;
            this.role = role;
            person = new Patient(firstName, lastName);
        }

        [BsonConstructor]
        public User(string username, string password, Person person, Role role) {
            this.username = username;
            this.password = password;
            this.role = role;
            this.person = person;
        }
    }
} 
