using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using HospitalSystem.Core;

namespace HospitalSystem.Tests;

public static class TestGenerator
{
    // THIS WILL DELETE YOUR EXISTING DATABASE!!
    public static void Generate(Hospital hospital)
    {
        // TODO: split to functions
        var dbClient = new MongoClient("mongodb://root:root@localhost:27017");  // TODO: unhardcode
        dbClient.DropDatabase("hospital");
        System.Console.WriteLine("DROPPED EXISTING DATABASE HOSPITAL");

        UserGenerator.GenerateUsers(hospital);
        RoomGenerator.GenerateRoomsAndEquipment(hospital);
        AppointmentGenerator.GenerateCheckupsAndOperations(hospital);
        AppointmentGenerator.GenerateCheckupChangeRequests(hospital);
        MedicationGenerator.GenerateMedication(hospital);
        MedicationGenerator.GenerateMedicationRequests(hospital);
        SurveyGenerator.GenerateSurveys(hospital);

        System.Console.WriteLine("GENERATED TESTS IN DB");

        WriteDatabaseToFile(dbClient);

        System.Console.WriteLine("WROTE TESTS TO FILE");
    }

    private static void WriteDatabaseToFile(MongoClient dbClient)
    {
        Dictionary<String, List<Object>> allCollections = new();
        foreach (string collectionName in dbClient.GetDatabase("hospital").ListCollectionNames().ToEnumerable())
        {
            var collection = dbClient.GetDatabase("hospital").GetCollection<BsonDocument>(collectionName).AsQueryable();
            allCollections[collectionName] = new();
            allCollections[collectionName].AddRange(collection);
        }
        File.WriteAllText("db/hospital.json", 
            allCollections.ToJson(
                new JsonWriterSettings {Indent = true}
            )
        );
    }
}