using MongoDB.Driver;
using MongoDB.Driver.Linq;
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

        public IMongoCollection<CheckupChangeRequest> GetAllCheckupChangeRequests()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<CheckupChangeRequest>("checkup_change_requests");
        }

        public IMongoQueryable<CheckupChangeRequest> GetCheckupChangeRequestsByState(RequestState state)
        {
            var requests = GetAllCheckupChangeRequests().AsQueryable();
            var matches = 
                from request in requests
                where request.RequestState == state
                select request;
            return matches;
        }

        public IMongoQueryable<CheckupChangeRequest> GetAllCheckupChangeRequestsAsQueryable()
        {
            //there might be a better way to do this
            return  GetAllCheckupChangeRequests().AsQueryable();
        }

        public void AddOrUpdateCheckupChangeRequest(Checkup checkup, CRUDOperation crudOperation, RequestState state = RequestState.PENDING)
        {
            var newRequest = new CheckupChangeRequest(checkup, crudOperation, state);
            var requests = GetAllCheckupChangeRequests();
            requests.ReplaceOne(request => request.Id == newRequest.Id, newRequest, new ReplaceOptions {IsUpsert = true});
        }

        public void AddOrUpdateCheckupChangeRequest(CheckupChangeRequest newRequest)
        {
            var requests = GetAllCheckupChangeRequests();
            requests.ReplaceOne(request => request.Id == newRequest.Id, newRequest, new ReplaceOptions {IsUpsert = true});
        }

        public void DeleteCheckupChangeRequest(CheckupChangeRequest request)
        {
         var requests = GetAllCheckupChangeRequests();
         var filter = Builders<CheckupChangeRequest>.Filter.Eq(deletedRequest => deletedRequest.Id, request.Id);
         requests.DeleteOne(filter);
        }
    }
}