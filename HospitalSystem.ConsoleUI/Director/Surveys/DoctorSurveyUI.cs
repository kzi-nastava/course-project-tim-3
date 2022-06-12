using HospitalSystem.Core;
using HospitalSystem.Core.Surveys;

namespace HospitalSystem.ConsoleUI.Director.Surveys;

public class DoctorSurveyUI : HospitalClientUI
{
    private List<DoctorSurvey> _doctorSurveys;

    public DoctorSurveyUI(Hospital hospital) : base(hospital)
    {
        _doctorSurveys = new();
    }

    private void RefreshSurveys()
    {
        _doctorSurveys = _hospital.SurveyService.GetAllDoctor().ToList();
    }


    public override void Start()
    {
        throw new NotImplementedException();
    }
}