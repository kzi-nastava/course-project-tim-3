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

        public IMongoCollection<Checkup> GetCheckups()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Checkup>("checkups");
        }

        public IMongoCollection<Operation> GetOperations()
        {
            return _dbClient.GetDatabase("hospital").GetCollection<Operation>("operations");
        }

        public void AddOrUpdateCheckup(Checkup newCheckup)
        {
            var checkups = GetCheckups();
            checkups.ReplaceOne(checkup => checkup.Id == newCheckup.Id, newCheckup, new ReplaceOptions {IsUpsert = true});
        }

        public void AddOrUpdateOperation(Operation newOperation)
        {
            var operations = GetOperations();
            operations.ReplaceOne(operation => operation.Id == newOperation.Id, newOperation, new ReplaceOptions {IsUpsert = true});
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
       

        public List<Checkup>  SearchPastCheckups(ObjectId patientId, string anamnesisKeyword)
        {
            var checkups = GetCheckups();
            //might not be the best way to indent
            List<Checkup> filteredCheckups = checkups.Find(
                                                    checkup => checkup.Anamnesis.ToLower().Contains(anamnesisKeyword.ToLower())
                                                    && checkup.StartTime < DateTime.Now 
                                                    && checkup.Patient.Id == patientId
                                                    ).ToList();
            return filteredCheckups;
        }

        public List<Checkup> GetFutureCheckupsByPatient(ObjectId id)
        {
            var checkups = GetCheckups();
            List<Checkup> patientCheckups = checkups.Find(checkup => checkup.StartTime > DateTime.Now && checkup.Patient.Id == id).ToList();
            return patientCheckups;
        }

        public List<Operation> GetOperationsByPatient(ObjectId id)
        {
            var operations = GetOperations();
            List<Operation> patientOperations = operations.Find(appointment => appointment.Patient.Id == id).ToList();
            return patientOperations;
        }
        
        public List<Operation> GetOperationsByDoctor(ObjectId id)
        {
            var operations = GetOperations();
            List<Operation> doctorsOperations = operations.Find(appointment => appointment.Doctor.Id == id).ToList();
            return doctorsOperations;
        }

        public List<Checkup> GetCheckupsByDay(DateTime date)
        {
            var checkups = GetCheckups();
            List<Checkup> checkupsByDay = checkups.Find(appointment => appointment.StartTime > date && appointment.StartTime < date.AddDays(1)).ToList();
            return checkupsByDay;
        }

        public List<Checkup> GetPastCheckupsByPatient(ObjectId id)
        {
            var checkups = GetCheckups();
            List<Checkup> selectedCheckups = checkups.Find(checkup => checkup.StartTime < DateTime.Now && checkup.Patient.Id == id).ToList();
            return selectedCheckups;
        }

        public Checkup GetCheckupById(ObjectId id)
        {
            var checkups = GetCheckups();
            Checkup checkup = checkups.Find(appointment => appointment.Id == id).FirstOrDefault();
            return checkup;
        }

        public void DeleteCheckup(Checkup checkup)
        {
         var checkups = GetCheckups();
         var filter = Builders<Checkup>.Filter.Eq(deletedCheckup => deletedCheckup.Id, checkup.Id);
         checkups.DeleteOne(filter);
        }

        public bool IsDoctorAvailable(DateTime date, Doctor doctor)
        {
            List<Checkup> checkups = GetCheckupsByDoctor(doctor.Id);
            List<Operation> operations = GetOperationsByDoctor(doctor.Id);
            foreach (Checkup checkup in checkups)
            {
                if (checkup.StartTime < date.AddMinutes(15) && date < checkup.EndTime)
                {
                    return false;
                } 
            }
            foreach (Operation operation in operations)
            {
                if (operation.StartTime < date.AddMinutes(15) && date < operation.EndTime)
                {
                    return false;
                } 
            }
            return true;
        }
    }
}