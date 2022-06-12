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

    public void AddResponse(Survey survey, SurveyResponse response)
    {
        survey.AddResponse(response);
        _repo.Replace(survey);
    }

    public IQueryable<HospitalSurvey> GetAllHospital()
    {
        return _repo.GetAllHospital();
    }

    public IQueryable<DoctorSurvey> GetAllDoctor()
    {
        return _repo.GetAllDoctor();
    }

    public IList<(Doctor, IEnumerable<DoctorSurveyResponse>)> GetResponsesGroupedByDoctor(DoctorSurvey survey)
    {
        return 
            (from pair in survey.GetResponsesGroupedByDoctor()
            select (_doctorService.GetById(pair.Item1), pair.Item2)).ToList();
    }

    public IList<HospitalSurvey> GetHospitalUnansweredBy(Person person)
    {
        return _repo.GetHospitalUnansweredBy(person);
    }

    public IEnumerable<(DoctorSurvey, IEnumerable<Doctor>)> GetDoctorUnansweredBy(Patient pat)
    {
        return 
            from notAnsweredSurveyDoctors in _repo.GetDoctorUnansweredBy(pat,   // TODO: think up a better name
                _appointmentService.GetAllAppointmentDoctors(pat))
            select (notAnsweredSurveyDoctors.Item1, 
                notAnsweredSurveyDoctors.Item2.Select(id => _doctorService.GetById(id)));
    }

    public IList<(Doctor, double?, int)> GetBestDoctors(DoctorSurvey survey, int count = 3)
    {
        return 
            (from res in survey.GetBestDoctors(count)  // TODO: think up a better name than res
            select (_doctorService.GetById(res.Item1), res.Item2, res.Item3)).ToList();
    }

    public IList<(Doctor, double?, int)> GetWorstDoctors(DoctorSurvey survey, int count = 3)
    {
        return 
            (from res in survey.GetWorstDoctors(count)
            select (_doctorService.GetById(res.Item1), res.Item2, res.Item3)).ToList();
    }
}