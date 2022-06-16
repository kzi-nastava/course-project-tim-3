using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem.Core;

public interface IAppointmentRepository
{
    public IMongoCollection<Checkup> GetCheckups();
    public IMongoCollection<Operation> GetOperations();
    public void UpsertCheckup(Checkup newCheckup);

    public void UpsertOperation(Operation newOperation);
    public void DeleteCheckup(Checkup checkup);
    public void DeleteOperation(Operation operation);
    public List<Operation> GetOperationsByDoctor(ObjectId id);
    public List<Operation> GetOperationsByPatient(ObjectId id);
    public List<Checkup> GetCheckupsByDoctor(Doctor doctor);
    public HashSet<ObjectId> GetAllAppointmentDoctors(Patient pat);
    public List<Operation> GetFutureOperationsByPatient(ObjectId id);
    public List<Checkup> GetFutureCheckupsByPatient(ObjectId id);
    public List<Checkup> SearchPastCheckups(ObjectId patientId, string anamnesisKeyword);


}
