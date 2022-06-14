using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem.Core.Medications.Requests;

public enum RequestStatus
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
    public RequestStatus Status { get; set; }

    public MedicationRequest(Medication requested, string directorComment)
    {
        Id = ObjectId.GenerateNewId();
        Requested = requested;
        DirectorComment = directorComment;
        DoctorComment = "/";
        Created = DateTime.Now;
        Status = RequestStatus.SENT;
    }

    [BsonConstructor]
    internal MedicationRequest(ObjectId id, Medication requested, string doctorComment, string directorComment,  
        DateTime created, RequestStatus status)
    {
        Id = id;
        Requested = requested;
        DoctorComment = doctorComment;
        DirectorComment = directorComment;
        Created = created;
        Status = status;
    }

    public override string ToString()
    {
        return  Requested.ToString() + "\nDate of creation: " + Created + "\nDirector comment: " + DirectorComment;
    }
}