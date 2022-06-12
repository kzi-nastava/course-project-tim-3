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

    public IQueryable<HospitalSurvey> GetAllHospital()
    {

        return GetAll().OfType<HospitalSurvey>();
    }

    public IQueryable<DoctorSurvey> GetAllDoctor()
    {
        return GetAll().OfType<DoctorSurvey>();
    }

    public IList<HospitalSurvey> GetHospitalUnansweredBy(Person person)
    {
        return
            (from survey in GetAllHospital().AsEnumerable()
            where !survey.WasAnsweredBy(person)
            select survey).ToList();
    }

    public IList<(DoctorSurvey, IEnumerable<ObjectId>)> GetDoctorUnansweredBy(Patient pat, HashSet<ObjectId> myDoctors)
    {
        return
            (from survey in GetAllDoctor().AsEnumerable()
            where myDoctors.Except(survey.AnsweredFor(pat)).Any()
            select (survey, myDoctors.Except(survey.AnsweredFor(pat)))).ToList();
    }
}