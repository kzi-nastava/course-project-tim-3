namespace HospitalSystem.Core.Surveys;

public class HospitalSurveyService
{
    private IHospitalSurveyRepository _repo;

    public HospitalSurveyService(IHospitalSurveyRepository repo)
    {
        _repo = repo;
    }

    public void Insert(HospitalSurvey survey)
    {
        _repo.Insert(survey);
    }

    public void AddResponse(HospitalSurvey survey, SurveyResponse response)
    {
        survey.AddResponse(response);
        _repo.Replace(survey);
    }

    public IQueryable<HospitalSurvey> GetAll()
    {
        return _repo.GetAll();
    }

    public IList<HospitalSurvey> GetUnansweredBy(Person person)
    {
        return _repo.GetUnansweredBy(person);
    }
}