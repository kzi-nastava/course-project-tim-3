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

    public void Upsert(Doctor newDoctor)
    {
        _doctorRepo.Upsert(newDoctor);
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

    
    public int CompareCheckupsByDoctorsName(Checkup checkup1, Checkup checkup2)
    {
        string name1 = GetById((ObjectId)checkup1.Doctor.Id).FirstName;
        string name2 = GetById((ObjectId)checkup2.Doctor.Id).FirstName;
        return String.Compare(name1, name2);
    }

    public int CompareCheckupsByDoctorsSpecialty(Checkup checkup1, Checkup checkup2)
    {
        string specialty1 = GetById((ObjectId)checkup1.Doctor.Id).Specialty.ToString();
        string specialty2 = GetById((ObjectId)checkup2.Doctor.Id).Specialty.ToString();
        return String.Compare(specialty1, specialty2);
    }

}