using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HospitalSystem.Core.Surveys;

[BsonKnownTypes(typeof(DoctorSurveyAnswer))]
public class SurveyAnswer
{
    public List<string?> Answers { get; set; }
    public List<int?> Ratings { get; set; }
    public ObjectId AnsweredBy { get; set; }

    public SurveyAnswer(List<string?> answers, List<int?> ratings, ObjectId answeredBy)
    {
        Answers = answers;
        Ratings = ratings;
        AnsweredBy = answeredBy;
    }

    public void Validate(Survey parent)
    {
        if (Ratings.Count != parent.RatingQuestions.Count)
        {
            throw new InvalidSurveyException("Wrong rating count for answer.");
        }
        if (Answers.Count != parent.Questions.Count)
        {
            throw new InvalidSurveyException("Wrong amount of answered questions for that answer.");
        }
        if (Ratings.Any(rating => rating is not null && (rating > 5 || rating < 0)))
        {
            throw new InvalidSurveyException("Rating must be between 1 and 5, inclusive");
        }
    }
}