using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

[System.Serializable]
public class InvalidSurveyException : System.Exception
{
    public InvalidSurveyException() { }
    public InvalidSurveyException(string message) : base(message) { }
    public InvalidSurveyException(string message, System.Exception inner) : base(message, inner) { }
    protected InvalidSurveyException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

[BsonKnownTypes(typeof(DoctorSurvey), typeof(HospitalSurvey))]
public abstract class Survey
{
    [BsonId]
    public ObjectId Id { get; }
    public string Title { get; set; }
    public List<string> Questions { get; set; }
    public List<string> RatingQuestions { get; set; }

    public Survey(List<string> questions, List<string> ratingQuestions, string title)
    {
        Id = ObjectId.GenerateNewId();
        Questions = questions;
        RatingQuestions = ratingQuestions;
        Title = title;
    }

    protected IEnumerable<(string, double?, int)> AggregateRatings(IEnumerable<SurveyResponse> responses)
    {
        return RatingQuestions.Select((question, i) => 
            (
                question, 
                (from response in responses
                select response.Ratings[i]).Average(),
                (from response in responses
                select response.Ratings[i]).Count(rating => rating != null)
            ));
    }

    protected void Validate(SurveyResponse response)
    {
        if (response.Ratings.Count != RatingQuestions.Count)
        {
            throw new InvalidSurveyException("Wrong rating count for response to that survey.");
        }
        if (response.Answers.Count != Questions.Count)
        {
            throw new InvalidSurveyException("Wrong amount of answered questions for that response.");
        }
        if (response.Ratings.Any(rating => rating is not null && (rating > 5 || rating < 0)))
        {
            throw new InvalidSurveyException("Rating must be between 1 and 5, inclusive");
        }
    }
}