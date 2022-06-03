// namespace HospitalSystem.Core;

// public class DoctorService 
// {
//     private DoctorRepository _doctorRepo;
     
//     public DoctorService(DoctorRepository doctorRepo)
//     {
//         _doctorRepo = doctorRepo;
//     }
//     public void CreateCheckup(DateTime dateTime, string name, string surname)
//     {
//         Patient patient = _hospital.PatientRepo.GetPatientByFullName(name,surname);
//     if (patient == null)
//     {
//         Console.WriteLine("No such patient existst.");
//         return false;
//     }
//     Doctor doctor = _hospital.DoctorRepo.GetById((ObjectId)_user.Person.Id);
//     Checkup checkup = new Checkup(dateTime, new MongoDBRef("patients", patient.Id), new MongoDBRef("doctors", _user.Person.Id), "anamnesis:");
//     if (_hospital.AppointmentRepo.IsDoctorAvailable(checkup.DateRange, doctor))
//     {
//         _hospital.AppointmentRepo.AddOrUpdateCheckup(checkup);
//         Console.WriteLine("\nCheckup successfully added");
//         return true;
//     }
//     else
//     {
//         Console.WriteLine("Doctor is not available at that time");
//         return false;
//     }
//     }
    
// }