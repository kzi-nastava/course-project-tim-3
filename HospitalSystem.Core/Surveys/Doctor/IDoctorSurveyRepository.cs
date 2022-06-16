using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public interface IDoctorSurveyRepository
{
    public void Insert(DoctorSurvey survey);

    public void Replace(DoctorSurvey survey);

    public IQueryable<DoctorSurvey> GetAll();

    public IList<(DoctorSurvey, IEnumerable<ObjectId>)> GetUnansweredBy(Patient pat, HashSet<ObjectId> myDoctors);
}