using MongoDB.Driver;
using MongoDB.Bson;

namespace Hospital
{
    public class AppointmentRepository
    {
        private MongoClient _dbClient;
        public AppointmentRepository(MongoClient _dbClient)
        {
            this._dbClient = _dbClient;
        }
        
        // public IMongoCollection<Appointment> GetAppointments()
        // {
        //     return _dbClient.GetDatabase("hospital").GetCollection<Appointment>("appointments");
        // }

        public IMongoCollection<Checkup> GetCheckups()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Checkup>("checkups");
        }

        public IMongoCollection<Operation> GetOperations()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Operation>("operations");
        }

        public void AddCheckup(DateTime timeAndDate, MongoDBRef patient, MongoDBRef doctor, TimeSpan duration, string anamnesis)
        {
            var newCheckup = new Checkup(timeAndDate, patient, doctor, anamnesis);
            var checkups = GetCheckups();
            var apCheck =  newCheckup;
            checkups.ReplaceOne(appointment => appointment.Id == apCheck.Id, apCheck, new ReplaceOptions {IsUpsert = true});
        }

        public void AddCheckup(Checkup newCheckup)
        {
            var checkups = GetCheckups();
            checkups.ReplaceOne(appointment => appointment.Id == newCheckup.Id, newCheckup, new ReplaceOptions {IsUpsert = true});
        }

        public void AddOperation(DateTime timeAndDate, MongoDBRef patient, MongoDBRef doctor, TimeSpan duration, string report)
        {
            var newOperation = new Operation(timeAndDate, patient, doctor, report);
            var operations = GetOperations();
            var apCheck = newOperation;
            operations.ReplaceOne(appointment => appointment.Id == apCheck.Id, apCheck, new ReplaceOptions {IsUpsert = true});
        }

        public List<Checkup> GetCheckupsByDoctor(ObjectId id)
        {
            var checkups = GetCheckups();
            List<Checkup> doctorsCheckups = checkups.Find(appointment => appointment.Doctor.Id == id).ToList();
            return doctorsCheckups;
        }

        public List<Checkup> GetCheckupsByPatient(ObjectId id)
        {
            var checkups = GetCheckups();
            List<Checkup> patientCheckups = checkups.Find(appointment => appointment.Patient.Id == id).ToList();
            return patientCheckups;
        }

        public List<Operation> GetOperationsByPatient(ObjectId id)
        {
            var operations = GetOperations();
            List<Operation> patientOperations = operations.Find(appointment => appointment.Doctor.Id == id).ToList();
            return patientOperations;
        }
        
        public List<Operation> GetOperationsByDoctor(ObjectId id)
        {
            var operations = GetOperations();
            List<Operation> doctorsOperations = operations.Find(appointment => appointment.Doctor.Id == id).ToList();
            return doctorsOperations;
        }

         public void AddOrUpdateCheckup(Checkup newCheckup)
        {
            var checkups = GetCheckups();
            checkups.ReplaceOne(checkup => checkup.Id == newCheckup.Id, newCheckup, new ReplaceOptions {IsUpsert = true});
        }
    }
}