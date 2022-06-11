namespace HospitalSystem.Core.Surveys;

public class SurveyService
{
    private ISurveyRepository _repo;
    private AppointmentService _appointmentService;

    public SurveyService(ISurveyRepository repo, AppointmentService appointmentService)
    {
        _repo = repo;
        _appointmentService = appointmentService;
    }

    public void Insert(Survey survey)
    {
        _repo.Insert(survey);
    }

    public void AddAnswer(Survey survey, SurveyAnswer answer)
    {
        survey.AddAnswer(answer);
        _repo.Replace(survey);
    }

    public IList<Survey> GetHospitalUnansweredBy(Person person)
    {
        return _repo.GetHospitalUnansweredBy(person);
    }

    public IList<Survey> GetDoctorUnansweredBy(Patient pat)
    {
        return _repo.GetDoctorUnansweredBy(pat, _appointmentService.GetAllAppointmentDoctors(pat));
    }
}