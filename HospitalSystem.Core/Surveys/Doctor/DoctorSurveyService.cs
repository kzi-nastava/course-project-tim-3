namespace HospitalSystem.Core.Surveys;

public record RatedDoctor(Doctor Doctor, double? Average, int Count);

public class DoctorSurveyService
{
    private IDoctorSurveyRepository _repo;
    private AppointmentService _appointmentService;
    private DoctorService _doctorService;

    public DoctorSurveyService(IDoctorSurveyRepository repo, AppointmentService appointmentService, DoctorService doctorService)
    {
        _repo = repo;
        _appointmentService = appointmentService;
        _doctorService = doctorService;
    }

    public void Insert(DoctorSurvey survey)
    {
        _repo.Insert(survey);
    }

    public void AddResponse(DoctorSurvey survey, SurveyResponse response, Doctor forDoctor)
    {
        survey.AddResponse(response, forDoctor.Id);
        _repo.Replace(survey);
    }

    public IQueryable<DoctorSurvey> GetAll()
    {
        return _repo.GetAll();
    }

    public IList<(Doctor, List<SurveyResponse>)> GetDoctorsWithResponsesFor(DoctorSurvey survey)
    {
        return 
            (from drResponse in survey.Responses
            select (_doctorService.GetById(drResponse.Key), drResponse.Value)).ToList();
    }

    public IEnumerable<(DoctorSurvey, IEnumerable<Doctor>)> GetUnansweredBy(Patient pat)
    {
        return 
            from notAnsweredSurveyDoctors in _repo.GetUnansweredBy(pat,   // TODO: think up a better name
                _appointmentService.GetAllAppointmentDoctors(pat))
            select (notAnsweredSurveyDoctors.Item1, 
                notAnsweredSurveyDoctors.Item2.Select(id => _doctorService.GetById(id)));
    }

    public IList<RatedDoctor> GetBestDoctors(DoctorSurvey survey, int count = 3)
    {
        return 
            (from drIdRatings in survey.GetBestDoctorIds(count)  // TODO: think up a better name than res
            where drIdRatings.Average != null
            select new RatedDoctor(_doctorService.GetById(drIdRatings.Id), drIdRatings.Average,
                drIdRatings.Count)).ToList();
    }

    public IList<RatedDoctor> GetWorstDoctors(DoctorSurvey survey, int count = 3)
    {
        return 
            (from drIdRatings in survey.GetWorstDoctorIds(count)
            where drIdRatings.Average != null
            select new RatedDoctor(_doctorService.GetById(drIdRatings.Id), drIdRatings.Average,
                drIdRatings.Count)).ToList();
    }
}