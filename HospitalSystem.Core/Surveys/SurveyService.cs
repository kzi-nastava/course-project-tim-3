namespace HospitalSystem.Core.Surveys;

public record RatedDoctor(Doctor Doctor, double? Average, int Count);

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

    public List<DoctorSurvey> GetByDoctor(Doctor doctor)
    {
        List<DoctorSurvey> filtered = new();
        var allDoctor = GetAllDoctor().ToList();

        if (allDoctor == null){
            return filtered;
        }

        foreach (var doctorSurvey in allDoctor)
        {
            if (doctorSurvey.Responses.ContainsKey(doctor.Id))
            {
                filtered.Add(doctorSurvey);
            }
        }
        return filtered;

    }

    public double GetAverageRatingDoctor(Doctor doctor)
    {   double sum = 0;
        int count = 0;
        var selectedSurveys = GetByDoctor(doctor);
        foreach (var drSurvey in selectedSurveys)
        {   
            var agregatedRatings = drSurvey.AggregateRatingsFor(doctor).ToList();
            foreach (var rating in agregatedRatings)
            {
                if (rating.Average != null){
                    sum += (double)rating.Average;
                    count++;
                }
            }
        }

        if (count==0)
        {
            return 10;
        }
        return sum/count;
    }
}