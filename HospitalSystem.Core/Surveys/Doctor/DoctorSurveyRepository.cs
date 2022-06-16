using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public class DoctorSurveyRepository : IDoctorSurveyRepository
{
    private MongoClient _dbClient;

    public DoctorSurveyRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<Survey> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Survey>("surveys");
    }

    public IQueryable<DoctorSurvey> GetAll()
    {
        return GetMongoCollection().AsQueryable().OfType<DoctorSurvey>();
    }

    public void Insert(DoctorSurvey survey)
    {
        GetMongoCollection().InsertOne(survey);
    }

    public void Replace(DoctorSurvey replacement)
    {
        GetMongoCollection().ReplaceOne(survey => survey.Id == replacement.Id, replacement);
    }

    public IList<(DoctorSurvey, IEnumerable<ObjectId>)> GetUnansweredBy(Patient pat, HashSet<ObjectId> myDoctors)
    {
        return
            (from survey in GetAll().AsEnumerable()
            where myDoctors.Except(survey.GetDoctorsRespondedToBy(pat)).Any()
            select (survey, myDoctors.Except(survey.GetDoctorsRespondedToBy(pat)))).ToList();
    }
}