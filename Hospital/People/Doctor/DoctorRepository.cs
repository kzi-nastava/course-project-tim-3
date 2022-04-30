using MongoDB.Driver;

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

        // this should return an empty list if there are no doctors in selected specialty
        public List<Doctor> GetDoctorBySpecialty(Specialty specialty)
        {
            //TODO: implement this
            return null;
        }
    }
}
