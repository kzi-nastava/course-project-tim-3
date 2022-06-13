using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

public interface IAppointmentRepository
{
    public IMongoCollection<Checkup> GetCheckups();
    public IMongoCollection<Operation> GetOperations();
    public void UpsertCheckup(Checkup newCheckup);

    public void UpsertOperation(Operation newOperation);
    public void DeleteCheckup(Checkup checkup);
    public void DeleteOperation(Operation operation);
}
