using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace HospitalSystem.Core;
public class DaysOffRequestRepository
{
    private MongoClient _dbClient;
        
    public DaysOffRequestRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }
    private IMongoCollection<DaysOffRequest> GetAll()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<DaysOffRequest>("days_off_requests");
    }

    public void Upsert(DaysOffRequest newRequest)
    {
        var requests = GetAll();
        requests.ReplaceOne(request => request.Id == newRequest.Id, newRequest, new ReplaceOptions {IsUpsert = true});
    }
}