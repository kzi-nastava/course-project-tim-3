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
public static class AppointmentGenerator
{
    public static void GenerateCheckupsAndOperations(Hospital hospital)
    {
        int i = 0;
        DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, 9, 15, 0);
        
        try
        {
            for (; i < 100; i++)
            {
                Random rand = new Random();
                var range = new DateRange(dateTime, dateTime.Add(new TimeSpan(0, 15, 0)), allowPast: true);
                Doctor doctor = hospital.DoctorService.GetByFullName("name" + ((i/16)*4 + 1),"surname" + ((i/16)*4 + 1));
                Patient patient = hospital.PatientService.GetByFullName("name" + ((i/16)*4 + 2),"surname" + ((i/16)*4 + 2));
                dateTime = dateTime.AddHours(3);

                if (i % 2 == 0)
                {   
                    Checkups(hospital, range, patient, doctor);
                    
                } else if (i % 2 == 1) 
                {
                    Operations(hospital, range, patient, doctor);
                }
            }
        }
        catch (NoAvailableRoomException e)
        {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine("Stopping appointment generation at " + i + " generated");
        }
    }

    private static void Checkups(Hospital hospital, DateRange range, Patient patient, Doctor doctor)
    {
        Checkup check = new Checkup(range, new MongoDBRef("patients",patient.Id),
            new MongoDBRef("doctors", doctor.Id), "anamneza");
        hospital.ScheduleService.ScheduleCheckup(check);
    }

    private static void Operations(Hospital hospital, DateRange range, Patient patient, Doctor doctor)
    {
        Operation op = new Operation(range, new MongoDBRef("patients",patient.Id),
            new MongoDBRef("doctors", doctor.Id), "report");
        hospital.ScheduleService.ScheduleOperation(op);
    }

    public static void GenerateCheckupChangeRequests(Hospital hospital)
    {
        for (int i = 0; i < 8; i++)
        {
            Doctor doctor = hospital.DoctorService.GetByFullName("name1","surname1");
            List<Checkup> checkups = hospital.AppointmentService.GetCheckupsByDoctor(doctor);

            if (i % 2 == 0)
            {   
                ApprovedChangeRequest(hospital, i, checkups);
            }
            else if (i % 2 == 1) 
            {
                DeniedChangeRequest(hospital, i , checkups);
            }    
        }
    }

    private static void ApprovedChangeRequest(Hospital hospital, int i, List<Checkup> checkups)
    {
        RequestState state = RequestState.PENDING;
        if (i % 4 == 0)
        {
            state = RequestState.APPROVED;
        }
        Checkup alteredCheckup = checkups[i];
        DateTime newDateAndTime =  new DateTime(2077,10,10);
        alteredCheckup.DateRange = new DateRange(newDateAndTime, newDateAndTime.Add(Checkup.DefaultDuration), true);
        CheckupChangeRequest request = new CheckupChangeRequest(alteredCheckup,CRUDOperation.UPDATE,state);
        hospital.CheckupChangeRequestService.Upsert(request);
    }

    private static void DeniedChangeRequest(Hospital hospital, int i, List<Checkup> checkups)
    {
        RequestState state = RequestState.PENDING;
        if (i % 3 == 0)
        {
            state = RequestState.DENIED;
        }
        CheckupChangeRequest request = new CheckupChangeRequest(checkups[i],CRUDOperation.DELETE,state);
        hospital.CheckupChangeRequestService.Upsert(request);
    }
}