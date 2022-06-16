using MongoDB.Driver;

namespace HospitalSystem.Core.Medications;

public class MedicationRepository
{
    private MongoClient _dbClient;

    public MedicationRepository(MongoClient _dbClient)
    {
        this._dbClient = _dbClient;
    }

    private IMongoCollection<Medication> GetMongoCollection()
    {
        return _dbClient.GetDatabase("hospital").GetCollection<Medication>("medications");
    }

    public void Upsert(Medication newMedication)
    {
        var medications = GetMongoCollection();
        medications.ReplaceOne(medication => medication.Id == newMedication.Id, newMedication, new ReplaceOptions {IsUpsert = true});
    }

    public Medication GetByName(string name)
    {
        var medications = GetMongoCollection();
        return medications.Find(medication => medication.Name == name).FirstOrDefault();
    }
}