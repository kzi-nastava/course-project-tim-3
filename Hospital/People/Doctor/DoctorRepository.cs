using MongoDB.Driver;
using MongoDB.Bson;

namespace Hospital
{
    public class DoctorRepository
    {
        private MongoClient _dbClient;

        public DoctorRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Doctor> GetDoctors()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Doctor>("doctors");
        }

        public void AddDoctor(string firstName, string lastName, Specialty specialty)
        {
            var newDoctor = new Doctor(firstName, lastName, specialty);
            var doctors = GetDoctors();
            doctors.ReplaceOne(doctor => doctor.Id == newDoctor.Id, newDoctor, new ReplaceOptions {IsUpsert = true});
        }

        public void AddDoctor(Doctor newDoctor)
        {
            var doctors = GetDoctors();
            doctors.ReplaceOne(doctor => doctor.Id == newDoctor.Id, newDoctor, new ReplaceOptions {IsUpsert = true});
        }

        // this should return an empty list if there are no doctors in selected specialty
        public List<Doctor> GetDoctorsBySpecialty(Specialty specialty)
        {
            var doctors = GetDoctors();
            var specizedDoctors =
                from doctor in doctors.AsQueryable<Doctor>()
                where doctor.Specialty == specialty
                select doctor;

            return specizedDoctors.ToList();
        }

        public Doctor GetDoctorBySpecialty(Specialty specialty)
        {
            var doctors = GetDoctors();
            var foundDoctor = doctors.Find(doctor => doctor.Specialty == specialty).FirstOrDefault();
            return foundDoctor;
        }

        public void AddOrUpdateDoctor(Doctor doctor)
        {
            var newDoctor = doctor;
            var doctors = GetDoctors();
            doctors.ReplaceOne(doctor => doctor.Id == newDoctor.Id, newDoctor, new ReplaceOptions {IsUpsert = true});
        }
        public Doctor GetDoctorByFullName(string firstName, string lastName)
        {
            var doctors = GetDoctors();
            var foundDoctor = doctors.Find(doctor => doctor.FirstName == firstName && doctor.LastName == lastName).FirstOrDefault();
            return foundDoctor;
        }

        public Doctor GetDoctorById(ObjectId id)
        {
            var doctors = GetDoctors();
            var foundDoctor = doctors.Find(doctor => doctor.Id == id).FirstOrDefault();
            return foundDoctor;
        }
    }
}
