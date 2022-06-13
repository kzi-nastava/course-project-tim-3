using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace HospitalSystem.Core.Surveys;

public class DoctorSurvey : Survey
{
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public  Dictionary<ObjectId, List<SurveyResponse>> Responses { get; set; }

    public DoctorSurvey(List<string> questions, List<string> ratingQuestions, string title)
        : base(questions, ratingQuestions, title)
    {
        Responses = new();
    }

    public IEnumerable<(string, double?, int)> AggregateRatingsFor(Doctor dr)
    {
        return AggregateRatings(Responses[dr.Id]);
    }

    public void AddResponse(SurveyResponse response, ObjectId forDoctor)
    {
        Validate(response);
        if (!Responses.ContainsKey(forDoctor))
        {
            Responses[forDoctor] = new();
        }
        Responses[forDoctor].Add(response);
    }

    public HashSet<ObjectId> GetDoctorsRespondedToBy(Patient pat)
    {
        return
            (from drResponse in Responses
            where drResponse.Value.Any(response => response.AnsweredBy == pat.Id)
            select drResponse.Key).ToHashSet();
    }

    // TODO: write best and worst in a better way, too similar functions
    public IEnumerable<(ObjectId, double?, int)> GetBestDoctors(int count)
    {
        return
            (from drResponse in Responses
            orderby drResponse.Value.Average(response => response.Ratings.Average()) descending
            select (
                drResponse.Key,
                drResponse.Value.Average(response => response.Ratings.Average()),
                drResponse.Value.Count(response => response.Ratings.Any(rating => rating != null)))).Take(count);
    }

    public IEnumerable<(ObjectId, double?, int)> GetWorstDoctors(int count)
    {
        return
            (from drResponse in Responses
            orderby drResponse.Value.Average(response => response.Ratings.Average()) ascending
            select (
                drResponse.Key,
                drResponse.Value.Average(response => response.Ratings.Average()),
                drResponse.Value.Count(response => response.Ratings.Any(rating => rating != null)))).Take(count);
    }
}