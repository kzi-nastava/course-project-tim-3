using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public class DoctorSurvey : Survey
{
    public List<DoctorSurveyResponse> Responses { get; set; }

    public DoctorSurvey(List<string> questions, List<string> ratingQuestions, string title)
        : base(questions, ratingQuestions, title)
    {
        Responses = new();
    }

    public override bool WasAnsweredBy(Person person)
    {
        return
            (from response in Responses
            where response.AnsweredBy == person.Id
            select response).Any();
    }

    public override void AddResponse(SurveyResponse response)
    {
        if (!(response is DoctorSurveyResponse))
        {
            throw new InvalidSurveyException("A doctor survey can only take doctor answers.");
        }
        var doctorResponse = (DoctorSurveyResponse) response;
        doctorResponse.Validate(this);
        Responses.Add(doctorResponse);
    }

    public HashSet<ObjectId> GetDoctorsRespondedToBy(Patient pat)
    {
        return
            (from response in Responses
            where response.AnsweredBy == pat.Id
            select ((DoctorSurveyResponse) response).DoctorId).ToHashSet();
    }

    // TODO: write best and worst in a better way, too similar functions. Better grouping maybe?
    public IEnumerable<(ObjectId, double?, int)> GetBestDoctors(int count)
    {
        return
            (from response in Responses
            group response by ((DoctorSurveyResponse) response).DoctorId into grp
            orderby grp.Average(response => response.Ratings.Average()) descending
            select (grp.Key, grp.Average(response => response.Ratings.Average()), grp.Count())).Take(count);

    }

    public IEnumerable<(ObjectId, double?, int)> GetWorstDoctors(int count)
    {
        return
            (from response in Responses
            group response by ((DoctorSurveyResponse) response).DoctorId into grp
            orderby grp.Average(response => response.Ratings.Average()) ascending
            select (grp.Key, grp.Average(response => response.Ratings.Average()), grp.Count())).Take(count);
    }
}