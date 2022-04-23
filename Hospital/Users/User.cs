using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Hospital
{
    enum Role
    {
        DOCTOR,
        PATIENT,
        SECRETARY,
        DIRECTOR
    }

    [BsonIgnoreExtraElements]
    class User
    {
        // todo: might want to add objectId here
        public string username {get;}
        public string password {get;}
        public Role role {get;}
        //private Person person;
        public User(string username, string password, Role role) {
            this.username = username;
            this.password = password;
            this.role = role;
        }
    }
} 
