using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public enum SurveyType
{
    HOSPITAL,
    DOCTOR
}

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

public class Survey
{
    [BsonId]
    public ObjectId Id { get; }
    public SurveyType Type { get; set; }
    public string Title { get; set; }
    public List<string> Questions { get; set; }
    public List<string> RatingQuestions { get; set; }
    [BsonElement]
    private List<SurveyAnswer> _answers;

    public Survey(List<string> questions, List<string> ratingQuestions, string title,
        SurveyType type = SurveyType.HOSPITAL)
    {
        Id = ObjectId.GenerateNewId();
        Questions = questions;
        RatingQuestions = ratingQuestions;
        Title = title;
        Type = type;
        _answers = new();
    }

    public void AddAnswer(SurveyAnswer answer)
    {
        if (answer.Ratings.Count != RatingQuestions.Count)
        {
            throw new InvalidSurveyException("Wrong rating count for answer.");
        }
        if (answer.Answers.Count != Questions.Count)
        {
            throw new InvalidSurveyException("Wrong amount of answered questions for that answer.");
        }
        if (answer.Ratings.Any(rating => rating is not null && (rating > 5 || rating < 0)))
        {
            throw new InvalidSurveyException("Rating must be between 1 and 5, inclusive");
        }
        if (answer.DoctorId == null && Type == SurveyType.DOCTOR)  // TODO: maybe diff classes
        {
            throw new InvalidSurveyException("Doctor survey must refer to a doctor");
        }
        if (answer.DoctorId != null && Type == SurveyType.HOSPITAL)
        {
            throw new InvalidSurveyException("Hospital survey must not refer to a doctor");
        }
        _answers.Add(answer);
    }
}