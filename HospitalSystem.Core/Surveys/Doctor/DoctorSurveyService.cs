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

    public List<DoctorSurvey> GetSpecificDoctorUnansweredBy(Patient pat, Doctor doc)
    {
        var allUnanswered = GetUnansweredBy(pat).ToList();
        List<DoctorSurvey> filteredUnanswered = new();
        foreach (var pair in allUnanswered)
        {
            foreach( var doctor in pair.Item2.ToList())
            {
                if (doctor.Id == doc.Id)
                {
                    filteredUnanswered.Add(pair.Item1);
                }
            }
        }
        return filteredUnanswered;
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
        var allDoctor = GetAll().ToList();

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