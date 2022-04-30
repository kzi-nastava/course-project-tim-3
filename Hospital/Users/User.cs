using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

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
        
        public string Email {get;set;}
        public string Password {get;set;}
        [BsonRepresentation(BsonType.String)]
        public Role Role {get;set;}
        public MongoDBRef Person {get;set;}

        public User(string email, string password, string firstName, string lastName, Role role)
        {
            this.Email = email;
            this.Password = password;
            this.Role = role;
            switch (role)
            {
                case Role.DOCTOR:
                {
                    Doctor doctor = new Doctor(firstName, lastName, Specialty.FAMILY_MEDICINE);
                    Person = new MongoDBRef("doctors",doctor.Id);
                    break;
                }
                case Role.PATIENT:
                {
                    Patient patient = new Patient(firstName, lastName, new MedicalRecord());
                    Person = new MongoDBRef("patients",patient.Id);
                    break;
                }
            }
        }

        [BsonConstructor]
        public User(string email, string password, Person person, Role role)
        {
            this.Email = email;
            this.Password = password;
            this.Role = role;
            switch (role)
            {
                case Role.DOCTOR:
                {
                    this.Person = new MongoDBRef("doctors",person.Id);
                    break;
                }
                case Role.PATIENT:
                {
                    this.Person = new MongoDBRef("patients",person.Id);
                    break;
                }
                case Role.DIRECTOR:
                    this.Person = new MongoDBRef("directors", person.Id);
                    break;
                case Role.SECRETARY:
                default:
                    this.Person = new MongoDBRef("secretaries", person.Id);
                    break;
            }
        }
    }
} 
