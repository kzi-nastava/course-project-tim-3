using MongoDB.Driver;
using MongoDB.Driver.Linq;
using HospitalSystem.Core.Medications.Requests;

namespace HospitalSystem.Core.DaysOff;
public class DaysOffRepository: IDaysOffRepository
{
    private MongoClient _dbClient;
        
    public DaysOffRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }
    private IMongoCollection<DaysOffRequest> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<DaysOffRequest>("days_off_requests");
    }

    public IQueryable<DaysOffRequest> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Upsert(DaysOffRequest newRequest)
    {
        var requests = GetMongoCollection();
        requests.ReplaceOne(request => request.Id == newRequest.Id, newRequest, new ReplaceOptions {IsUpsert = true});
    }

    public void UpdateStatus(DaysOffRequest request, RequestStatus status)
    {
        var requestsGet = GetMongoCollection();
        request.Status = status;
        requestsGet.ReplaceOne(req => req.Id == request.Id , request, new ReplaceOptions {IsUpsert = true} );
    }
    
    public void UpdateExplanation(DaysOffRequest request, string explanation)
    {
        var requestsGet = GetMongoCollection();
        request.Explanation = explanation;
        requestsGet.ReplaceOne(req => req.Id == request.Id , request, new ReplaceOptions {IsUpsert = true} );
    }
    
    public IQueryable<DaysOffRequest> GetAllOnPending()
    {
        return 
            from request in GetAll()
            where request.Status == RequestStatus.SENT
            select request;
    }
}