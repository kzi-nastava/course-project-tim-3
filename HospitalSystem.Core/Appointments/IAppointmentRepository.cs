using MongoDB.Driver;

namespace HospitalSystem.Core;

public interface IAppointmentRepository
{
    public IMongoCollection<Checkup> GetCheckups();
    public IMongoCollection<Operation> GetOperations();
    public void UpsertCheckup(Checkup newCheckup);

    public void UpsertOperation(Operation newOperation);
    public void DeleteCheckup(Checkup checkup);
}
