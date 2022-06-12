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

    public IList<HospitalSurvey> GetHospitalUnansweredBy(Person person)
    {
        return _repo.GetHospitalUnansweredBy(person);
    }

    public IEnumerable<(DoctorSurvey, IEnumerable<Doctor>)> GetDoctorUnansweredBy(Patient pat)
    {
        return 
            from notAnsweredSurveyDoctors in _repo.GetDoctorUnansweredBy(pat, 
                _appointmentService.GetAllAppointmentDoctors(pat))
            select (notAnsweredSurveyDoctors.Item1, 
                notAnsweredSurveyDoctors.Item2.Select(id => _doctorService.GetById(id)));
    }

    public IList<(Doctor, double?, int)> GetBestDoctors(DoctorSurvey survey, int count = 3)
    {
        return 
            (from res in survey.GetBestDoctors(survey, count)
            select (_doctorService.GetById(res.Item1), res.Item2, res.Item3)).ToList();
    }

    public IList<(Doctor, double?, int)> GetWorstDoctors(DoctorSurvey survey, int count = 3)
    {
        return 
            (from res in survey.GetWorstDoctors(survey, count)
            select (_doctorService.GetById(res.Item1), res.Item2, res.Item3)).ToList();
    }
}