using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public interface ISurveyRepository
{
    public void Insert(Survey survey);

    public void Replace(Survey survey);

    public IList<Survey> GetHospitalUnansweredBy(Person person);

    public IList<Survey> GetDoctorUnansweredBy(Person person, HashSet<ObjectId> myDoctors);

    public IList<(ObjectId, int, int)> GetBestDoctors(Survey survey, int count);

    public IList<(ObjectId, int, int)> GetWorstDoctors(Survey survey, int count);
}