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
        return _doctorRepo.GetManyBySpecialty(specialty);
    }

    public List<Doctor> GetManyBySpecialty(string keyword)
    {
        return _doctorRepo.GetManyBySpecialty(keyword);
    }

    public List<Doctor> GetManyByName(string keyword)
    {
        return _doctorRepo.GetManyByName(keyword);
    }

    public List<Doctor> GetManyByLastName(string keyword)
    {
        return _doctorRepo.GetManyByLastName(keyword);
    }

    public Doctor GetOneBySpecialty(Specialty specialty)
    {
        return _doctorRepo.GetOneBySpecialty(specialty);
    }

    public Doctor GetByFullName(string firstName, string lastName)
    {
        return _doctorRepo.GetByFullName(firstName, lastName);
    }

    public Doctor GetById(ObjectId id)
    {
        return _doctorRepo.GetById(id);
    }
}