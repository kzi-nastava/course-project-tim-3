using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalSystem.Core.Surveys;

[BsonKnownTypes(typeof(DoctorSurveyResponse))]
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

    public void Validate(Survey parent)
    {
        if (Ratings.Count != parent.RatingQuestions.Count)
        {
            throw new InvalidSurveyException("Wrong rating count for response to that survey.");
        }
        if (Answers.Count != parent.Questions.Count)
        {
            throw new InvalidSurveyException("Wrong amount of answered questions for that response.");
        }
        if (Ratings.Any(rating => rating is not null && (rating > 5 || rating < 0)))
        {
            throw new InvalidSurveyException("Rating must be between 1 and 5, inclusive");
        }
    }
}