namespace HospitalSystem.Core.Equipment;

public class EquipmentAmount
{
    public string Name {get; set;}

    public int Amount{get; set;}

    public EquipmentAmount(string name, int amount)
    {
        Name = name;
        Amount = amount;
    }
}