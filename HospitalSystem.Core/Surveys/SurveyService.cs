namespace HospitalSystem.Core.Surveys;

public class SurveyService
{
    private ISurveyRepository _repo;
    private AppointmentService _appointmentService;
    private DoctorService _doctorService;

    public SurveyService(ISurveyRepository repo, AppointmentService appointmentService, DoctorService doctorService)
    {
        _repo = repo;
        _appointmentService = appointmentService;
        _doctorService = doctorService;
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

    public IList<(Doctor, int, int)> GetBestDoctors(Survey survey, int count = 3)
    {
        return 
            (from res in _repo.GetBestDoctors(survey, count)
            select (_doctorService.GetById(res.Item1), res.Item2, res.Item3)).ToList();
    }

    public IList<(Doctor, int, int)> GetWorstDoctors(Survey survey, int count = 3)
    {
        return 
            (from res in _repo.GetWorstDoctors(survey, count)
            select (_doctorService.GetById(res.Item1), res.Item2, res.Item3)).ToList();
    }
}