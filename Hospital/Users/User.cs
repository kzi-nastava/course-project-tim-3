using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Hospital;

public enum Role
{
    DOCTOR,
    PATIENT,
    SECRETARY,
    DIRECTOR
}

public enum Block
{
    UNBLOCKED,
    BY_SYSTEM,
    BY_SECRETARY,
}

[BsonIgnoreExtraElements]
public class User
{
    // todo: might want to add objectId here
    [BsonId]
    public ObjectId Id { get; set; }
    public string Email {get;set;}
    public string Password {get;set;}
    [BsonRepresentation(BsonType.String)]
    public Role Role {get;set;}
    [BsonRepresentation(BsonType.String)]
    public Block BlockStatus {get;set;}
    public MongoDBRef Person {get;set;}

    public User(string email, string password, Person person, Role role, Block blockStatus = Block.UNBLOCKED)
    {
        Id = ObjectId.GenerateNewId();
        Email = email;
        Password = password;
        Role = role;
        BlockStatus = blockStatus;
        switch (role)
        {
            case Role.DOCTOR:
            {
                Person = new MongoDBRef("doctors",person.Id);
                break;
            }
            case Role.PATIENT:
            {
                Person = new MongoDBRef("patients",person.Id);
                break;
            }
            case Role.DIRECTOR:
                Person = new MongoDBRef("directors", person.Id);
                break;
            case Role.SECRETARY:
            default:
                Person = new MongoDBRef("secretaries", person.Id);
                break;
        }
    }
}
