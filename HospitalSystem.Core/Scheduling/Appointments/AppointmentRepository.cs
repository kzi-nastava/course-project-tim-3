using MongoDB.Driver;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Core;

[System.Serializable]
public class NoAvailableRoomException : System.Exception
{
    public NoAvailableRoomException() { }
    public NoAvailableRoomException(string message) : base(message) { }
    public NoAvailableRoomException(string message, System.Exception inner) : base(message, inner) { }
    protected NoAvailableRoomException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class AppointmentRepository : IAppointmentRepository
{
    private MongoClient _dbClient;

    public AppointmentRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }

    public IMongoCollection<Checkup> GetCheckups()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Checkup>("checkups");
    }

     public IMongoCollection<Checkup> GetCheckupsByDoctor()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Checkup>("checkups");
    }

    public IMongoCollection<Operation> GetOperations()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Operation>("operations");
    }

    public void UpsertCheckup(Checkup newCheckup)
    {
        var checkups = GetCheckups();
        checkups.ReplaceOne(checkup => checkup.Id == newCheckup.Id, newCheckup, new ReplaceOptions {IsUpsert = true});
    }

    public void UpsertOperation(Operation newOperation)
    {
        var operations = GetOperations();
        operations.ReplaceOne(operation => operation.Id == newOperation.Id, newOperation, new ReplaceOptions {IsUpsert = true});
    }

    public void DeleteCheckup(Checkup checkup)
    {
        var checkups = GetCheckups();
        var filter = Builders<Checkup>.Filter.Eq(deletedCheckup => deletedCheckup.Id, checkup.Id);
        checkups.DeleteOne(filter);
    }

    public void DeleteOperation(Operation operation)
    {
        var operations = GetOperations();
        var filter = Builders<Operation>.Filter.Eq(deletedOperation => deletedOperation.Id, operation.Id);
        operations.DeleteOne(filter);
    }

    public HashSet<ObjectId> GetAllAppointmentDoctors(Patient pat)
    {
        return
            (from checkup in GetCheckups().AsQueryable()
            select (ObjectId) checkup.Doctor.Id)
            .Union(
            from operation in GetOperations().AsQueryable()
            select (ObjectId) operation.Doctor.Id).ToHashSet();
    }
    
    public List<Operation> GetOperationsByDoctor(ObjectId id)
    {
        var operations = GetOperations();
        List<Operation> doctorsOperations = operations.Find(appointment => appointment.Doctor.Id == id).ToList();
        return doctorsOperations;
    }

    public List<Operation> GetOperationsByPatient(ObjectId id)
    {
        var operations = GetOperations();
        List<Operation> patientOperations = operations.Find(appointment => appointment.Patient.Id == id).ToList();
        return patientOperations;
    }

    public List<Checkup> GetCheckupsByDoctor(Doctor doctor)
    {
        var checkups = GetCheckups();
        return
            (from checkup in checkups.AsQueryable()
            where doctor.Id == checkup.Doctor.Id
            select checkup).ToList();
    }
}