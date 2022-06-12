namespace HospitalSystem.Core.Surveys;

public class HospitalSurvey : Survey
{
    public List<SurveyResponse> Responses { get; set; }

    public HospitalSurvey(List<string> questions, List<string> ratingQuestions, string title)
        : base(questions, ratingQuestions, title)
    {
        Responses = new();
    }

    public override void AddResponse(SurveyResponse response)
    {
        response.Validate(this);  // TODO: might want to move validation to survey, so you don't pass this
        Responses.Add(response);
    }

    public bool WasAnsweredBy(Person person)
    {
        return
            (from response in Responses
            where response.AnsweredBy == person.Id
            select response).Any();
    }

    public IEnumerable<(string, double?, int)> AggregateRatings()
    {
        return RatingQuestions.Select((question, i) => 
            (
                question, 
                (from response in Responses
                select response.Ratings[i]).Average(),
                (from response in Responses
                select response.Ratings[i]).Count()
            ));
    }
}