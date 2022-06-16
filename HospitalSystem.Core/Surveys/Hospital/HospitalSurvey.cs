namespace HospitalSystem.Core.Surveys;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class HospitalSurvey : Survey
{
    public List<SurveyResponse> Responses { get; set; }
    
    [BsonConstructor]
    public HospitalSurvey(List<string> questions, List<string> ratingQuestions, string title)
        : base(questions, ratingQuestions, title)
    {
        Responses = new();
    }

    public void AddResponse(SurveyResponse response)
    {
        Validate(response);
        Responses.Add(response);
    }

    public bool WasAnsweredBy(Person person)
    {
        return
            (from response in Responses
            where response.AnsweredBy == person.Id
            select response).Any();
    }

    public IEnumerable<AggregatedRating> AggregateRatings()
    {
        return AggregateRatings(Responses);
    }
}