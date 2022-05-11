using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Hospital
{
    public class AppointmentChangeRequest
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public MongoDBRef AppointmentToChange { get; set; }

        public MongoDBRef Doctor { get; set; }

        public DateTime StartTime { get; set; }

        AppointmentChangeRequest()
        {
            
        }

    }


}
