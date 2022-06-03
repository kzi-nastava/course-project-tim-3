using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem.Core
{
    public class DoctorRepository
    {
        private MongoClient _dbClient;

        public DoctorRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Doctor> GetAll()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Doctor>("doctors");
        }

        // this should return an empty list if there are no doctors in selected specialty
        public List<Doctor> GetManyBySpecialty(Specialty specialty)
        {
            var doctors = GetAll();
            var specizedDoctors =
                from doctor in doctors.AsQueryable<Doctor>()
                where doctor.Specialty == specialty
                select doctor;

            return specizedDoctors.ToList();
        }
        //this is not so nice but it works
         public List<Doctor> GetManyBySpecialty(string keyword)
        {
            var doctors = GetAll();
            var allDoctors =
                from doctor in doctors.AsQueryable<Doctor>()
                select doctor;

            List<Doctor> filteredDoctors = allDoctors.ToList().FindAll(doctor => doctor.Specialty.ToString().Contains(keyword.ToUpper()));
            return filteredDoctors;
        }

        public List<Doctor> GetManyByName(string keyword)
        {
            var doctors = GetAll();
            var allDoctors =
                from doctor in doctors.AsQueryable<Doctor>()
                select doctor;

            return allDoctors.ToList().FindAll(doctor => doctor.FirstName.Contains(keyword));
        }

         public List<Doctor> GetManyByLastName(string keyword)
        {
            var doctors = GetAll();
            var allDoctors =
                from doctor in doctors.AsQueryable<Doctor>()
                select doctor;

            List<Doctor> filteredDoctors = allDoctors.ToList().FindAll(doctor => doctor.LastName.Contains(keyword));
            return filteredDoctors;
        }

        public Doctor GetOneBySpecialty(Specialty specialty)
        {
            var doctors = GetAll();
            var foundDoctor = doctors.Find(doctor => doctor.Specialty == specialty).FirstOrDefault();
            return foundDoctor;
        }

        public void AddOrUpdateDoctor(Doctor doctor)
        {
            var newDoctor = doctor;
            var doctors = GetAll();
            doctors.ReplaceOne(doctor => doctor.Id == newDoctor.Id, newDoctor, new ReplaceOptions {IsUpsert = true});
        }
        public Doctor GetByFullName(string firstName, string lastName)
        {
            var doctors = GetAll();
            var foundDoctor = doctors.Find(doctor => doctor.FirstName == firstName && doctor.LastName == lastName).FirstOrDefault();
            return foundDoctor;
        }

        public Doctor GetById(ObjectId id)
        {
            var doctors = GetAll();
            var foundDoctor = doctors.Find(doctor => doctor.Id == id).FirstOrDefault();
            return foundDoctor;
        }
    }
}
