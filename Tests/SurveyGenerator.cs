using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using HospitalSystem.Core;
using HospitalSystem.Core.Rooms;
using HospitalSystem.Core.Equipment;
using HospitalSystem.Core.Surveys;
using HospitalSystem.Core.Medications;
using HospitalSystem.Core.Medications.Requests;
using HospitalSystem.Core.Utils;

namespace HospitalSystem.Tests;
public static class SurveyGenerator
{
    public static void GenerateSurveys(Hospital hospital)
    {
        HospitalSurveys(hospital);
        DoctorSurveys(hospital);
        hospital.HospitalSurveyService.Insert(new HospitalSurvey(new List<string> {"Opininion2?"},
            new List<string>{"Overall2"}, "Hospital2"));
        hospital.DoctorSurveyService.Insert(new DoctorSurvey(new List<string> {"Opininion2?"},
            new List<string>{"Overall2"}, "Doctor2"));
    }

    public static void HospitalSurveys(Hospital hospital)
    {
        var hospitalSurvey = new HospitalSurvey(new List<string> {"Opininion?"},
            new List<string>{"Overall"}, "Hospital1");

        hospital.HospitalSurveyService.Insert(hospitalSurvey);
        hospital.HospitalSurveyService.AddResponse(hospitalSurvey,
            new SurveyResponse(new List<string?>{null}, new List<int?>{4},
            hospital.PatientService.GetByFullName("name2", "surname2").Id));
        hospital.HospitalSurveyService.AddResponse(hospitalSurvey,
            new SurveyResponse(new List<string?>{"Bad hospital! Hate it!"}, new List<int?>{5},
            hospital.PatientService.GetByFullName("name6", "surname6").Id));
    }

    public static void DoctorSurveys(Hospital hospital)
    {
        var doctorSurvey = new DoctorSurvey(new List<string> {"Opininion?"},
            new List<string>{"Overall"}, "Doctor1");
        hospital.DoctorSurveyService.Insert(doctorSurvey);
        hospital.DoctorSurveyService.AddResponse(doctorSurvey,
            new SurveyResponse(new List<string?>{"Good good"}, new List<int?>{null},
                hospital.PatientService.GetByFullName("name2", "surname2").Id),
                hospital.DoctorService.GetOneBySpecialty(Specialty.STOMATOLOGY));
        hospital.DoctorSurveyService.AddResponse(doctorSurvey,
            new SurveyResponse(new List<string?>{"Very Good good"}, new List<int?>{4},
                hospital.PatientService.GetByFullName("name6", "surname6").Id),
                hospital.DoctorService.GetOneBySpecialty(Specialty.DERMATOLOGY));
        hospital.DoctorSurveyService.AddResponse(doctorSurvey,
            new SurveyResponse(new List<string?>{"Very much Good good"}, new List<int?>{3},
                hospital.PatientService.GetByFullName("name10", "surname10").Id),
                hospital.DoctorService.GetOneBySpecialty(Specialty.DERMATOLOGY));
        hospital.DoctorSurveyService.AddResponse(doctorSurvey,
            new SurveyResponse(new List<string?>{"BAD Doctor"}, new List<int?>{1},
                hospital.PatientService.GetByFullName("name2", "surname2").Id),
                hospital.DoctorService.GetOneBySpecialty(Specialty.RADIOLOGY));
    }
}