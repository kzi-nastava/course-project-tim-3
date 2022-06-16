namespace HospitalSystem.Core.Surveys;

public interface IHospitalSurveyRepository
{
    public void Insert(HospitalSurvey survey);

    public void Replace(HospitalSurvey survey);

    public IQueryable<HospitalSurvey> GetAll();

    public IList<HospitalSurvey> GetUnansweredBy(Person person);
}