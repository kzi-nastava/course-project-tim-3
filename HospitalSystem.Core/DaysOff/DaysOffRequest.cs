using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using HospitalSystem.Core.Utils;
using HospitalSystem.Core.Medications.Requests;

namespace HospitalSystem.Core;

public class DaysOffRequest
{
    [BsonId]
    public ObjectId Id { get; }
    [BsonElement]
    public Doctor Doctor { get; }
    [BsonElement]
    public string Reason { get; }
    [BsonElement]
    public string Explanation { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    [BsonElement]
    public DateTime Created { get; }
    [BsonElement]
    public DateRange DaysOff { get; }
    [BsonRepresentation(BsonType.String)]
    public RequestStatus Status { get; set; }

    public DaysOffRequest(Doctor doctor, string reason, DateRange daysOff, string explanation = "")
    {
        Id = ObjectId.GenerateNewId();
        Created = DateTime.Now;
        Status = RequestStatus.SENT;
        Doctor = doctor;
        Reason = reason;
        Explanation = explanation;
        DaysOff = daysOff;
    }

    [BsonConstructor]
    internal DaysOffRequest(ObjectId id, Doctor doctor, string reason, DateTime created, DateRange daysOff, RequestStatus status, string explanation)
    {
        Id = id;
        Doctor = doctor;
        Reason = reason;
        Explanation = explanation;
        Created = created;
        DaysOff = daysOff;
        Status = status;
    }

    public override string ToString()
    {
        return  Doctor.FirstName + " " + Doctor.LastName + " requested to have the following days off: from" + DaysOff.Starts.ToShortDateString() + 
        " to " + DaysOff.Ends.ToShortDateString() + "\nfor following reason: " + Reason + "; Status: " + Status + "\nexplenation: " + Explanation;
    }
}