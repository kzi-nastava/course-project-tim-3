using MongoDB.Bson.Serialization.Attributes;

namespace Hospital;
[BsonIgnoreExtraElements]
public class MedicalRecord {
    [BsonElement]
    public double HeightInCm {get; set;}
    [BsonElement]
    public double WeightInKg {get; set;}
    [BsonElement]
    public List<string> AnamnesisHistory {get;} 
    [BsonElement]
    public List<string> Allergies {get;} 
    [BsonConstructor]
    public MedicalRecord() 
    {
        HeightInCm = 180.0;
        WeightInKg = 70.0;
        AnamnesisHistory = new List<string>();
        Allergies = new List<string>();
    }
    [BsonConstructor]
    public MedicalRecord(double heightInCm, double weightInKg, List<string> anamnesisHistory, List<string> allergies)
    {
        HeightInCm = heightInCm;
        WeightInKg = weightInKg;
        AnamnesisHistory = anamnesisHistory;
        Allergies = allergies;
    }
    public override string ToString()
    {
        return "Height In Cm : " + HeightInCm + "\nWeight In Cm : " + WeightInKg + "\nAnamnesis History : " 
        + string.Join( ",", AnamnesisHistory) + "\nAllergies : " + string.Join( ",", Allergies);
    }
}