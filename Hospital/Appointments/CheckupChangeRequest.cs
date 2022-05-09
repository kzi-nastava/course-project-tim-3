using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Hospital
{
    public class CheckupChangeRequest
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public MongoDBRef CheckupToChange { get; set; }

        public Checkup UpdatedCheckup { get; set; }

        public CRUDOperation CRUDOperation { get; set; }

        public CheckupChangeRequest(Checkup checkupToChange, Checkup updatedCheckup, CRUDOperation crudOperation)
        {
            Id = ObjectId.GenerateNewId();
            CheckupToChange = new MongoDBRef("checkup_change_requests",checkupToChange.Id);
            UpdatedCheckup = updatedCheckup;
            CRUDOperation = crudOperation;
        }

    }


}
