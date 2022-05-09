using MongoDB.Driver;
using MongoDB.Bson;

namespace Hospital
{
    public class CheckupChangeRequestRepository
    {
        private MongoClient _dbClient;
        public CheckupChangeRequestRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<CheckupChangeRequest> GetCheckupChangeRequests()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<CheckupChangeRequest>("checkup_change_requests");
        }

        public void AddOrUpdateCheckupChangeRequest(Checkup checkupToChange, Checkup updatedCheckup, CRUDOperation crudOperation)
        {
            var newRequest = new CheckupChangeRequest(checkupToChange, updatedCheckup, crudOperation);
            var requests = GetCheckupChangeRequests();
            requests.ReplaceOne(request => request.Id == newRequest.Id, newRequest, new ReplaceOptions {IsUpsert = true});
        }

        public void AddOrUpdateCheckupChangeRequest(CheckupChangeRequest newRequest)
        {
            var requests = GetCheckupChangeRequests();
            requests.ReplaceOne(request => request.Id == newRequest.Id, newRequest, new ReplaceOptions {IsUpsert = true});
        }

        public void DeleteCheckupChangeRequest(CheckupChangeRequest request)
        {
         var requests = GetCheckupChangeRequests();
         var filter = Builders<CheckupChangeRequest>.Filter.Eq(deletedRequest => deletedRequest.Id, request.Id);
         requests.DeleteOne(filter);
        }
    }
}