using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace HospitalSystem.Core
{
    public class CheckupChangeRequestRepository : ICheckupChangeRequestRepository
    {
        private MongoClient _dbClient;
        
        public CheckupChangeRequestRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<CheckupChangeRequest> GetMongoCollection()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<CheckupChangeRequest>("checkup_change_requests");
        }

        public IQueryable<CheckupChangeRequest> GetAll()
        {
            return GetMongoCollection().AsQueryable();
        }

        public IQueryable<CheckupChangeRequest> GetByState(RequestState state)
        {
            var requests = GetAll();
            var matches = 
                from request in requests
                where request.RequestState == state
                select request;
            return matches;
        }

        public List<CheckupChangeRequest> GetCheckUpChangeRequests()
        {
            var requestsGet = GetAll();
            List<CheckupChangeRequest> requests = new List<CheckupChangeRequest>();
            var matchingRequests = from request in requestsGet.AsQueryable() select request;

            foreach(var p in matchingRequests){
                    requests.Add(p);
            }   
            return requests;
        }

        public void AddOrUpdate(CheckupChangeRequest newRequest)
        {
            var requests = GetMongoCollection();
            requests.ReplaceOne(request => request.Id == newRequest.Id, newRequest, new ReplaceOptions {IsUpsert = true});
        }            


        public void Delete(CheckupChangeRequest request)
        {
         var requests = GetMongoCollection();
         var filter = Builders<CheckupChangeRequest>.Filter.Eq(deletedRequest => deletedRequest.Id, request.Id);
         requests.DeleteOne(filter);
        }

        public void UpdateRequest(int indexId, RequestState state)
        {   
            List<CheckupChangeRequest> requests = GetAll().ToList();
            requests.RemoveAll(u => u.RequestState != RequestState.PENDING);
            var request = requests[indexId];
            var requestsGet = GetMongoCollection();
            request.RequestState = state;
            requestsGet.ReplaceOne(req => req.Id == request.Id , request, new ReplaceOptions {IsUpsert = true} );
        }
    }
}