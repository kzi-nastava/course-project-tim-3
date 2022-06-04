using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem.Core;

public class DoctorService 
{
    private DoctorRepository _doctorRepo;
     
    public DoctorService(DoctorRepository doctorRepo)
    {
        _doctorRepo = doctorRepo;
    }

    public IMongoCollection<Doctor> GetAll()
    {
        return _doctorRepo.GetAll();
    }

    public List<Doctor> GetManyBySpecialty(Specialty specialty)
    {
        var doctors = _doctorRepo.GetAll();
            var specizedDoctors =
                from doctor in doctors.AsQueryable<Doctor>()
                where doctor.Specialty == specialty
                select doctor;

            return specizedDoctors.ToList();
    }

    public Doctor GetOneBySpecialty(Specialty specialty)
    {
        var doctors = _doctorRepo.GetAll();
        var foundDoctor = doctors.Find(doctor => doctor.Specialty == specialty).FirstOrDefault();
        return foundDoctor;
    }

    public void Upsert(Doctor doctor)
    {
        _doctorRepo.Upsert(doctor);
    }

    public Doctor GetByFullName(string firstName, string lastName)
    {
        var doctors = _doctorRepo.GetAll();
        var foundDoctor = doctors.Find(doctor => doctor.FirstName == firstName && doctor.LastName == lastName).FirstOrDefault();
        return foundDoctor;
    }

    public Doctor GetById(ObjectId id)
    {
        var doctors = _doctorRepo.GetAll();
        var foundDoctor = doctors.Find(doctor => doctor.Id == id).FirstOrDefault();
        return foundDoctor;
    }
}