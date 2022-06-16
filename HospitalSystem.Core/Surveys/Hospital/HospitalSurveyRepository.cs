using MongoDB.Driver;

namespace HospitalSystem.Core.Surveys;

public class HospitalSurveyRepository : IHospitalSurveyRepository
{
    private MongoClient _dbClient;

    public HospitalSurveyRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<Survey> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Survey>("surveys");
    }

    public IQueryable<HospitalSurvey> GetAll()
    {
        return GetMongoCollection().AsQueryable().OfType<HospitalSurvey>();
    }

    public void Insert(HospitalSurvey survey)
    {
        GetMongoCollection().InsertOne(survey);
    }

    public void Replace(HospitalSurvey replacement)
    {
        GetMongoCollection().ReplaceOne(survey => survey.Id == replacement.Id, replacement);
    }

    public IList<HospitalSurvey> GetUnansweredBy(Person person)
    {
        return
            (from survey in GetAll().AsEnumerable()
            where !survey.WasAnsweredBy(person)
            select survey).ToList();
    }
}