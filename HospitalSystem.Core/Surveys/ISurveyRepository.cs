using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public interface ISurveyRepository
{
    public void Insert(Survey survey);

    public void Replace(Survey survey);

    public IList<Survey> GetHospitalUnansweredBy(Person person);

    public IList<(Survey, IEnumerable<ObjectId>)> GetDoctorUnansweredBy(Patient pat, HashSet<ObjectId> myDoctors);
}