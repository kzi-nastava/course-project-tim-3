using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public class DoctorSurveyAnswer : SurveyAnswer
{
    public ObjectId DoctorId { get; set; }

    public DoctorSurveyAnswer(List<string?> answers, List<int?> ratings, ObjectId answeredBy, ObjectId doctorId)
        : base(answers, ratings, answeredBy)
    {
        DoctorId = doctorId;
    }
}