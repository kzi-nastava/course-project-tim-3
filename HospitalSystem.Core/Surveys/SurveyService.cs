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

    public void AddResponse(HospitalSurvey survey, SurveyResponse response)
    {
        survey.AddResponse(response);
        _repo.Replace(survey);
    }

    public void AddResponse(DoctorSurvey survey, SurveyResponse response, Doctor forDoctor)
    {
        survey.AddResponse(response, forDoctor.Id);
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

    public IList<(Doctor, List<SurveyResponse>)> GetDoctorsWithResponsesFor(DoctorSurvey survey)
    {
        return 
            (from drResponse in survey.Responses
            select (_doctorService.GetById(drResponse.Key), drResponse.Value)).ToList();
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
            (from drIdRatings in survey.GetBestDoctorIds(count)  // TODO: think up a better name than res
            where drIdRatings.Item2 != null
            select (_doctorService.GetById(drIdRatings.Item1), drIdRatings.Item2, drIdRatings.Item3)).ToList();
    }

    public IList<(Doctor, double?, int)> GetWorstDoctors(DoctorSurvey survey, int count = 3)
    {
        return 
            (from drIdRatings in survey.GetWorstDoctorIds(count)
            where drIdRatings.Item2 != null
            select (_doctorService.GetById(drIdRatings.Item1), drIdRatings.Item2, drIdRatings.Item3)).ToList();
    }
}