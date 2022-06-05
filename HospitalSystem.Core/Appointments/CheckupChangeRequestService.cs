using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace HospitalSystem.Core;

public class CheckupChangeRequestService
{
    private ICheckupChangeRequestRepository _requestRepo;

    public CheckupChangeRequestService(CheckupChangeRequestRepository requestRepo)
    {
    _requestRepo = requestRepo;
    }

    public IMongoCollection<CheckupChangeRequest> GetAll()
    {
        return _requestRepo.GetAll();
    }

    public IMongoQueryable<CheckupChangeRequest> GetByState(RequestState state)
    {
        return _requestRepo.GetByState(state);
    }

    public IMongoQueryable<CheckupChangeRequest> GetAllAsQueryable()
    {
        return  _requestRepo.GetAll().AsQueryable();
    }
    public List<CheckupChangeRequest> GetCheckUpChangeRequests()
    {
        return _requestRepo.GetCheckUpChangeRequests();
    }

    public void AddOrUpdate(CheckupChangeRequest newRequest)
    {
        _requestRepo.AddOrUpdate(newRequest);
    }

    public void Delete(CheckupChangeRequest request)
    {
        _requestRepo.Delete(request);
    }


    public void UpdateRequest(int indexId, RequestState state)
    {   
        _requestRepo.UpdateRequest(indexId, state);
    }
}