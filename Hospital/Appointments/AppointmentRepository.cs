using MongoDB.Driver;

namespace Hospital
{
    public class AppointmentRepository
    {
        private MongoClient _dbClient;

        public AppointmentRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }

        public IMongoCollection<Appointment> GetAppointments()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Appointment>("appointments");
        }
    }
}