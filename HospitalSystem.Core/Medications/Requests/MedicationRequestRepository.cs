using MongoDB.Driver;

namespace HospitalSystem.Core.Medications.Requests;

public class MedicationRequestRepository : IMedicationRequestRepository
{
    private MongoClient _dbClient;

    public MedicationRequestRepository(MongoClient dbClinet)
    {
        _dbClient = dbClinet;
    }

    private IMongoCollection<MedicationRequest> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<MedicationRequest>("medication_requests");
    }

    public void Insert(MedicationRequest request)
    {
        GetMongoCollection().InsertOne(request);
    }

    public void Replace(MedicationRequest replacement)
    {
        GetMongoCollection().ReplaceOne(request => request.Id == replacement.Id, replacement);
    }

    public IQueryable<MedicationRequest> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public IQueryable<MedicationRequest> GetSent()
    {
        return
            from req in GetAll()
            where req.Status == RequestStatus.SENT
            select req;
    }

    public IQueryable<MedicationRequest> GetDenied()
    {
        return
            from req in GetAll()
            where req.Status == RequestStatus.DENIED
            select req;
    }
}