using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

public interface IAppointmentRepository
{
    public IMongoCollection<Checkup> GetCheckups();
    public IMongoCollection<Operation> GetOperations();
    public void AddOrUpdateCheckup(Checkup newCheckup);

    public void AddOrUpdateOperation(Operation newOperation);
    public void DeleteCheckup(Checkup checkup);
}
