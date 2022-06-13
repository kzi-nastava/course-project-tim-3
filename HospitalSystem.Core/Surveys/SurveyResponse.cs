using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public class SurveyResponse
{
    public List<string?> Answers { get; set; }
    public List<int?> Ratings { get; set; }
    public ObjectId AnsweredBy { get; set; }

    public SurveyResponse(List<string?> answers, List<int?> ratings, ObjectId answeredBy)
    {
        Answers = answers;
        Ratings = ratings;
        AnsweredBy = answeredBy;
    }
}