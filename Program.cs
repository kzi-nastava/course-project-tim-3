using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace Hospital
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MongoClient _dbClient = new MongoClient("mongodb://root:root@localhost:27017");
            var hospital = new Hospital();
            var hospitalUsers = new {Users = new List<User>()};

            // //generate tests TODO: move this to dedicated teting interface

            // //generate users

            // for (int i = 0; i < 100; i++)
            // {
            //     User user;
            //     if (i % 4 == 0)
            //         user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.DIRECTOR);
            //     else if (i % 4 == 1)
            //     {
            //         user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.DOCTOR);
            //         Doctor doctor;
            //         doctor = new Doctor("imenko" + i,"prezimenic" + i, Specialty.FAMILY_MEDICINE);
            //         hospital.DoctorRepo.AddOrUpdateDoctor(doctor);
            //     }
            //     else if (i % 4 == 2) 
            //     {
            //         Patient patient;
            //         patient = new Patient("imenko" + i, "prezimenic" + i, new MedicalRecord());
            //         hospital.PatientRepo.AddOrUpdatePatient(patient);
            //         user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.PATIENT);
            //     }  
            //     else
            //         user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.SECRETARY);
            // hospital.UserRepo.AddUser(user.Email, user.Password, user.Person.FirstName, user.Person.LastName, user.Role);
            // hospitalUsers.Users.Add(user);
            // File.WriteAllText("db/hospital.json", hospitalUsers.ToBsonDocument().ToJson(
            //    new JsonWriterSettings {Indent = true}));
            // }
            // //generate checkups and operations
            // DateTime dateTime = new DateTime(2022, 5, 1, 4, 15, 0);
            // for (int i = 0; i < 100; i++)
            // {
            //     Doctor doctor = hospital.DoctorRepo.GetDoctorByName("imenko1");
            //     Patient patient = hospital.PatientRepo.GetPatientByName("imenko2");
            //     dateTime = dateTime.AddHours(1);

            //     if (i % 2 == 0)
            //     {   
            //         Checkup check = new Checkup(dateTime, new MongoDBRef("patients",patient.Id), new MongoDBRef("doctors", doctor.Id), "anamneza");
            //         hospital.AppointmentRepo.AddOrUpdateCheckup(check.TimeAndDate, check.Patient, check.Doctor, check.Duration, check.Anamnesis);
            //     } else if (i % 2 == 1) 
            //     {
            //         Operation op = new Operation(dateTime, new MongoDBRef("patients",patient.Id), new MongoDBRef("doctors", doctor.Id), "report");
            //         hospital.AppointmentRepo.AddOrUpdateOperation(op.TimeAndDate, op.Patient, op.Doctor, op.Duration, op.Report);
            //     }    
            // }

            // List<Checkup> doctorCheckups = hospital.AppointmentRepo.GetCheckupsByDoctor(hospital.DoctorRepo.GetDoctorByName("imenko1").Id);
            // foreach (Checkup c in doctorCheckups)
            // {
            //     Console.WriteLine(c.toString());
            // }
            // DateTime date = new DateTime(2022, 5, 2, 13, 10, 0);
            // Console.Write(hospital.AppointmentRepo.IsDoctorBusy(date,hospital.DoctorRepo.GetDoctorByName("imenko1")));
            // var ui = new DoctorUI(hospital);
            // ui.Start();
        }
    }
}