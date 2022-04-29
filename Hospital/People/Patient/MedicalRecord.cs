namespace Hospital;

public class MedicalRecord {
    double HeightInCm {get; set;}
    double WeightInKg {get; set;}
    List<string> AnamnesisHistory {get;} 
    List<string> Allergies {get;} 
    public MedicalRecord() 
    {
        HeightInCm = 180.0;
        WeightInKg = 70.0;
        AnamnesisHistory = new List<string>();
        Allergies = new List<string>();
    }
    public MedicalRecord(float heightInCm, float weightInKg, List<string> anamnesisHistory, List<string> allergies)
    {
        HeightInCm = heightInCm;
        WeightInKg = weightInKg;
        AnamnesisHistory = anamnesisHistory;
        Allergies = allergies;
    }
    public string toString()
    {
        return "Height In Cm : " + HeightInCm + "\nWeight In Cm : " + WeightInKg + "\nAnamnesis";
    }
}