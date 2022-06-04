using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;
using MongoDB.Driver.Linq;

namespace HospitalSystem.Core;

public interface ICheckupChangeRequestRepository{
    public IMongoCollection<CheckupChangeRequest> GetAll();

    public IMongoQueryable<CheckupChangeRequest> GetByState(RequestState state);
    
    public List<CheckupChangeRequest> GetCheckUpChangeRequests();

    public void AddOrUpdate(CheckupChangeRequest newRequest);

    public void Delete(CheckupChangeRequest request);

    public void UpdateRequest(int indexId, RequestState state);
}
