using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace HospitalSystem.Core.Surveys;

public record RatedDoctorId(ObjectId Id, double? Average, int Count);

public class DoctorSurvey : Survey
{
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
    public  Dictionary<ObjectId, List<SurveyResponse>> Responses { get; set; }  // TODO: naming?

    public DoctorSurvey(List<string> questions, List<string> ratingQuestions, string title)
        : base(questions, ratingQuestions, title)
    {
        Responses = new();
    }

    public IEnumerable<AggregatedRating> AggregateRatingsFor(Doctor dr)
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

    public IEnumerable<RatedDoctorId> GetBestDoctorIds(int count)
    {
        return GetBestDoctorIds().Take(count);
    }

    private IEnumerable<RatedDoctorId> GetBestDoctorIds()
    {
        return GetWorstDoctorIds().Reverse();
    }

    private IEnumerable<RatedDoctorId> GetWorstDoctorIds()
    {
        return
            (from drResponse in Responses
            orderby drResponse.Value.Average(response => response.Ratings.Average()) ascending
            select new RatedDoctorId(
                drResponse.Key,
                drResponse.Value.Average(response => response.Ratings.Average()),
                drResponse.Value.Count(response => response.Ratings.Any(rating => rating != null))));
    }

    public IEnumerable<RatedDoctorId> GetWorstDoctorIds(int count)
    {
        return GetWorstDoctorIds().Take(count);
    }

}