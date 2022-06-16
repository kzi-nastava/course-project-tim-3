using MongoDB.Driver;

namespace HospitalSystem.Core;

public class CheckupChangeRequestService
{
    private ICheckupChangeRequestRepository _requestRepo;

    public CheckupChangeRequestService(CheckupChangeRequestRepository requestRepo)
    {
    _requestRepo = requestRepo;
    }

    public IQueryable<CheckupChangeRequest> GetAll()
    {
        return _requestRepo.GetAll();
    }

    public IQueryable<CheckupChangeRequest> GetByState(RequestState state)
    {
        return _requestRepo.GetByState(state);
    }

    public IQueryable<CheckupChangeRequest> GetAllAsQueryable()
    {
        return  _requestRepo.GetAll().AsQueryable();
    }
    public List<CheckupChangeRequest> GetCheckUpChangeRequests()
    {
        return _requestRepo.GetCheckUpChangeRequests();
    }

    public void Upsert(CheckupChangeRequest newRequest)
    {
        _requestRepo.Upsert(newRequest);
    }

    public void Delete(CheckupChangeRequest request)
    {
        _requestRepo.Delete(request);
    }


    public void UpdateRequest(CheckupChangeRequest request, RequestState state)
    {   
        _requestRepo.UpdateRequest(request, state);
    }
}