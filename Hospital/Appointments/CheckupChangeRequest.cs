using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Hospital
{

    public enum RequestState
    {
        PENDING,
        DENIED,
        APPROVED,
    }
    public class CheckupChangeRequest
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public MongoDBRef CheckupToChange { get; set; }

        public Checkup UpdatedCheckup { get; set; }
        
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public CRUDOperation CRUDOperation { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public RequestState RequestState { get; set; }
        
        public CheckupChangeRequest(Checkup checkupToChange, Checkup updatedCheckup, CRUDOperation crudOperation, RequestState requestState = RequestState.PENDING)
        {
            Id = ObjectId.GenerateNewId();
            CheckupToChange = new MongoDBRef("checkups",checkupToChange.Id);
            UpdatedCheckup = updatedCheckup;
            CRUDOperation = crudOperation;
            RequestState = requestState;
        }

        //TODO: Implement better tostring
        public override string ToString ()
        {
            return CheckupToChange + " " + CRUDOperation + " " +RequestState;
        }
    }


}
