using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

// REVIEW: Use namespaces
// Entity
// Repository
// Service
// Model
//      CLI
//      GUI


namespace Hospital
{
    public enum CRUDOperation
    {
        CREATE,
        UPDATE,
        DELETE,
        READ
    }

    [BsonIgnoreExtraElements]
    public class CheckupChangeLog {
        public DateTime StartTime {get; set;}
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public CRUDOperation CRUDOperation {get; set;}

        public CheckupChangeLog(DateTime startTime, CRUDOperation crudOperation) 
        {
            StartTime = startTime;
            CRUDOperation = crudOperation;
        }
        public override string ToString()
        {
            return StartTime + " " + CRUDOperation.ToString();
        }
    
    }
}