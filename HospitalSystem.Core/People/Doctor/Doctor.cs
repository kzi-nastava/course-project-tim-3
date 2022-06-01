using MongoDB.Bson.Serialization.Attributes;
namespace HospitalSystem.Core
{
    public enum Specialty
    {
        DERMATOLOGY,
        RADIOLOGY,
        STOMATOLOGY,
        OPHTHALMOLOGY,
        FAMILY_MEDICINE
    }
    public class Doctor : Person
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Specialty Specialty {get; set;}
        
        public Doctor(string firstName, string lastName, Specialty specialty) : base(firstName, lastName)
        {
            Specialty = specialty;
        }
        
        public override string ToString() 
        {
            return FirstName +" "+ LastName +" "+ Specialty;
        }
    }
}