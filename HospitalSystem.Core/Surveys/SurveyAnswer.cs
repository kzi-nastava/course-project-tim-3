using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public class SurveyAnswer
{
    [BsonId]
    public ObjectId Id { get; }
    public List<string?> Answers { get; set; }
    public List<int?> Ratings { get; set; }
    [BsonIgnoreIfNull]
    public ObjectId? DoctorId { get; set; }
    public ObjectId AnsweredBy { get; set; }

    public SurveyAnswer(List<string?> answers, List<int?> ratings)
    {
        Id = ObjectId.GenerateNewId();
        Answers = answers;
        Ratings = ratings;
    }
}