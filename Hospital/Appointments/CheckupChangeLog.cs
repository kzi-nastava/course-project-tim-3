using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

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
        public DateTime TimeAndDate {get; set;}
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public CRUDOperation CRUDOperation {get; set;}

        public CheckupChangeLog(DateTime timeAndDate, CRUDOperation crudOperation) 
        {
            TimeAndDate = timeAndDate;
            CRUDOperation = crudOperation;
        }
        public override string ToString()
        {
            return TimeAndDate + " " + CRUDOperation.ToString();
        }
    
    }
}