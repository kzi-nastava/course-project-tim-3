using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalSystem.Core
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
        public Checkup Checkup { get; set; }
        
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public CRUDOperation CRUDOperation { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public RequestState RequestState { get; set; }
        
        public CheckupChangeRequest(Checkup checkup, CRUDOperation crudOperation, RequestState requestState = RequestState.PENDING)
        {
            Id = ObjectId.GenerateNewId();
            Checkup = checkup;
            CRUDOperation = crudOperation;
            RequestState = requestState;
        }

        //TODO: Implement better tostring
        public override string ToString ()
        {
            return Checkup + " " + CRUDOperation + " " + RequestState;
        }
    }


}
