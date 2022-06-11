using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public class SurveyRepository : ISurveyRepository
{
    private MongoClient _dbClient;

    public SurveyRepository(MongoClient dbClient)
    {
        _dbClient = dbClient;
    }

    private IMongoCollection<Survey> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Survey>("surveys");
    }

    private IQueryable<Survey> GetAll()
    {
        return GetMongoCollection().AsQueryable();
    }

    public void Insert(Survey survey)
    {
        GetMongoCollection().InsertOne(survey);
    }

    public void Replace(Survey replacement)
    {
        GetMongoCollection().ReplaceOne(survey => survey.Id == replacement.Id, replacement);
    }

    public IList<Survey> GetHospitalUnansweredBy(Person person)
    {
        return
            (from survey in GetAll().AsEnumerable()
            where survey.Type == SurveyType.HOSPITAL
            && !survey.WasAnsweredBy(person)
            select survey).ToList();
    }

    public IList<(Survey, IEnumerable<ObjectId>)> GetDoctorUnansweredBy(Patient pat, HashSet<ObjectId> myDoctors)
    {
        return
            (from survey in GetAll().AsEnumerable()
            where survey.Type == SurveyType.DOCTOR
            && myDoctors.Except(survey.AnsweredFor(pat)).Any()
            select (survey, myDoctors.Except(survey.AnsweredFor(pat)))).ToList();
    }
}