using MongoDB.Bson;

namespace HospitalSystem.Core.Surveys;

public class DoctorSurveyResponse : SurveyResponse
{
    public ObjectId DoctorId { get; set; }

    public DoctorSurveyResponse(List<string?> answers, List<int?> ratings, ObjectId answeredBy, ObjectId doctorId)
        : base(answers, ratings, answeredBy)
    {
        DoctorId = doctorId;
    }
}