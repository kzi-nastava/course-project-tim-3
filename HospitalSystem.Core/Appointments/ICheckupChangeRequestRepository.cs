using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;
using MongoDB.Driver.Linq;

namespace HospitalSystem.Core;

public interface ICheckupChangeRequestRepository{
    public IQueryable<CheckupChangeRequest> GetAll();

    public IQueryable<CheckupChangeRequest> GetByState(RequestState state);
    
    public List<CheckupChangeRequest> GetCheckUpChangeRequests();

    public void AddOrUpdate(CheckupChangeRequest newRequest);

    public void Delete(CheckupChangeRequest request);

    public void UpdateRequest(CheckupChangeRequest request, RequestState state);
}
