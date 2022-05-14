using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace HospitalSystem
{
    public class CheckupChangeRequestRepository
    {
        private MongoClient _dbClient;
        public CheckupChangeRequestRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<CheckupChangeRequest> GetAll()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<CheckupChangeRequest>("checkup_change_requests");
        }

        public IMongoQueryable<CheckupChangeRequest> GetByState(RequestState state)
        {
            var requests = GetAll().AsQueryable();
            var matches = 
                from request in requests
                where request.RequestState == state
                select request;
            return matches;
        }

        public IMongoQueryable<CheckupChangeRequest> GetAllAsQueryable()
        {
            //there might be a better way to do this
            return  GetAll().AsQueryable();
        }

        public void AddOrUpdate(CheckupChangeRequest newRequest)
        {
            var requests = GetAll();
            requests.ReplaceOne(request => request.Id == newRequest.Id, newRequest, new ReplaceOptions {IsUpsert = true});
        }

        public void Delete(CheckupChangeRequest request)
        {
         var requests = GetAll();
         var filter = Builders<CheckupChangeRequest>.Filter.Eq(deletedRequest => deletedRequest.Id, request.Id);
         requests.DeleteOne(filter);
        }
    }
}