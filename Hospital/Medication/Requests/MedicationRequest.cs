using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem;

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
    [BsonElement]
    public DateTime Created { get; }
    [BsonRepresentation(BsonType.String)]
    public MedicationRequestStatus Status { get; set; }

    public MedicationRequest(Medication requested, string directorComment)
    {
        Id = ObjectId.GenerateNewId();
        Requested = requested;
        DirectorComment = directorComment;
        DoctorComment = "";
        Created = DateTime.Now;
        Status = MedicationRequestStatus.SENT;
    }
}