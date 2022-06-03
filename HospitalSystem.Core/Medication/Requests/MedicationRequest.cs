using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem.Core;

public enum MedicationRequestStatus
{
    APPROVED,
    DENIED,
    SENT
}

public class MedicationRequest
{
    [BsonId]
    public ObjectId Id { get; }
    public Medication Requested { get; set; }
    public string DoctorComment { get; set; }
    public string DirectorComment { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    [BsonElement]
    public DateTime Created { get; }
    [BsonRepresentation(BsonType.String)]
    public MedicationRequestStatus Status { get; set; }

    public MedicationRequest(Medication requested, string directorComment)
    {
        Id = ObjectId.GenerateNewId();
        Requested = requested;
        DirectorComment = directorComment;
        DoctorComment = "/";
        Created = DateTime.Now;
        Status = MedicationRequestStatus.SENT;
    }

    [BsonConstructor]
    internal MedicationRequest(ObjectId id, Medication requested, string doctorComment, string directorComment,  
        DateTime created, MedicationRequestStatus status)
    {
        Id = id;
        Requested = requested;
        DoctorComment = doctorComment;
        DirectorComment = directorComment;
        Created = created;
        Status = status;
    }
}