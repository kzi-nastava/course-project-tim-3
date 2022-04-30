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

            // generate tests TODO: move this to dedicated teting interface


            // generate users

            // for (int i = 0; i < 100; i++)
            // {
            //     User user;
            //     if (i % 4 == 0)
            //         user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.DIRECTOR);
            //     else if (i % 4 == 1)
            //     {
            //         user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.DOCTOR);
            //         Doctor doctor;
            //         doctor = new Doctor("imenko" + i,"prezimenic" + i, "Family medicine");
            //         hospital.DoctorRepo.AddDoctor(doctor);
            //     }
            //     else if (i % 4 == 2) 
            //     {
            //         Patient patient;
            //         patient = new Patient("imenko" + i, "prezimenic" + i, new MedicalRecord());
            //         hospital.PatientRepo.AddPatient(patient);
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
            
            // for (int i = 0; i < 100; i++)
            // {
            //     Doctor doctor = hospital.DoctorRepo.GetDoctorByName("imenko1");
            //     Patient patient = hospital.PatientRepo.GetPatientByName("imenko11");
            //     if (i % 2 == 0)
            //     {   
            //         Checkup check = new Checkup(DateTime.Now, new MongoDBRef("patients",patient.Id), new MongoDBRef("doctors", doctor.Id), "anamneza");
            //         hospital.AppointmentRepo.AddCheckup(check.TimeAndDate, check.Patient, check.Doctor, check.Duration, check.Anamnesis);
            //     } else if (i % 2 == 1) 
            //     {
            //         Operation op = new Operation(DateTime.Now, new MongoDBRef("patients",patient.Id), new MongoDBRef("doctors", doctor.Id), "report");
            //         hospital.AppointmentRepo.AddOperation(op.TimeAndDate, op.Patient, op.Doctor, op.Duration, op.Report);
            //     }    
            // }

            // List<Checkup> doctorCheckups = hospital.AppointmentRepo.GetCheckupsByDoctor(hospital.DoctorRepo.GetDoctorByName("imenko1").Id);
            // foreach (Checkup c in doctorCheckups)
            // {
            //     Console.WriteLine(c.toString());
            // }
     
            //var hospitalContents = new {Users = new List<User>()};
            //for (int i = 0; i < 100; i++)
            //{
            //    User user;
            //    if (i % 4 == 0)
            //        user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.DIRECTOR);
            //    else if (i % 4 == 1)
            //        user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.DOCTOR);
            //    else if (i % 4 == 2)
            //        user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.PATIENT);
            //    else
            //        user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.SECRETARY);
            //    ui.AddUser(user.Email, user.Password, user.Person, user.Role);
            //    hospitalContents.Users.Add(user);
            //    File.WriteAllText("db/hospital.json", hospitalContents.ToBsonDocument().ToJson(
            //        new JsonWriterSettings {Indent = true}));
            //}
            //ui.Start();
        }
    }
}