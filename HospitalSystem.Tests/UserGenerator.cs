using HospitalSystem.Core;

namespace HospitalSystem.Tests;

public static class UserGenerator
{
    public static void GenerateUsers(Hospital hospital)
    {
        User user;
        int doctorSpecialtynumber = 0;
        for (int i = 0; i < 100; i++)
        {
            if (i % 4 == 0)
            {
                user = Directors(hospital, i);
            }
            else if (i % 4 == 1)
            {
                user = Doctors(hospital, i, doctorSpecialtynumber);
            }
            else if (i % 4 == 2) 
            {
                user = Patients(hospital, i);
            }  
            else
            {
                user = Secretaries(hospital, i);
            }
            hospital.UserService.Upsert(user);     
            doctorSpecialtynumber++;            
        }
    }

    private static User Directors(Hospital hospital, int i)
    {
        var director = new Person("name" + i, "surname" + i);
        hospital.PersonRepo.Insert(director);
        return new User("a" + i, "a" + i, director, Role.DIRECTOR);
    }

    private static User Doctors(Hospital hospital, int i, int doctorSpecialtynumber)
    {
        int namesCount = Enum.GetNames(typeof(Specialty)).Length;
        // counts on derma being first
        Specialty doctorsSpecialty = (Specialty)(doctorSpecialtynumber%namesCount+Specialty.DERMATOLOGY);
        var doctor = new Doctor("name" + i,"surname" + i, doctorsSpecialty);
        hospital.DoctorService.Upsert(doctor);
        return new User("a" + i, "a" + i, doctor, Role.DOCTOR);
    }

    private static User Patients(Hospital hospital, int i)
    {
        var patient = new Patient("name" + i, "surname" + i, new MedicalRecord());
        hospital.PatientService.Upsert(patient);
        return new User("a" + i, "a" + i, patient, Role.PATIENT);
    }

    private static User Secretaries(Hospital hospital, int i)
    {
        var secretary = new Person("name" + i, "surname" + i);
        hospital.PersonRepo.Insert(secretary);
        return new User("a" + i, "a" + i, secretary, Role.SECRETARY);
    }
}