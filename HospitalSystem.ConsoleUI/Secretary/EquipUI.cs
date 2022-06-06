// using MongoDB.Driver;
// using MongoDB.Bson;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;
// [System.Serializable]

public class EquipUI : ConsoleUI
{   
    public EquipUI(Hospital _hospital) : base(_hospital) {}

    public override void Start()
    {
        while (true)
        {
            try
            {
                System.Console.Clear();
                System.Console.WriteLine("INPUT OPTIONS:");
                System.Console.WriteLine("1. Procure equipment-(pe)");
                System.Console.WriteLine("2. Move equipment-(me)");
                System.Console.WriteLine("3. Quit-(q)");
                System.Console.WriteLine("4. Exit-(x)");
                System.Console.Write(">> ");
                var choice = ReadSanitizedLine();
                if (choice == "procure equipment" || choice == "pe")
                {
                    ProcureEquipment();
                }
                else if(choice == "move equipment" || choice == "me")
                {
                    MoveEquipment();
                }
                else if(choice == "quit" || choice == "q")
                {
                    continue;
                }
                else if (choice == "exit" || choice == "x")
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    System.Console.Write("UNRECOGNIZED OPTION. INPUT ANYTHING TO CONTINUE >> ");
                    ReadSanitizedLine();
                }
            }
            catch (QuitToMainMenuException)
            {

            }
        }
    }
    public void ProcureEquipment()
    {
        System.Console.Clear();

        List<EquipmentAmount> emptyEquipments = _hospital.EquipmentService.GetEmpty();
        System.Console.WriteLine("Out of stock: ");
        ShowEmptyEquipment(emptyEquipments);

        System.Console.Write("\nEnter the number of equipment: ");
        var number = ReadInt();
        if(number >= emptyEquipments.Count() || number < 0)
        {
            throw new InvalidInputException("That number does not exist");
        }

        System.Console.Write("Enter the amount: ");
        var ammount = ReadInt();
        if(ammount <= 0)
        {
            throw new InvalidInputException("Ammount cant be negative or zero");
        }

        System.Console.Write("Enter the equipment type (checkup, operation, furniture, hallway): ");
        var equipmentType = ReadSanitizedLine();
        bool success = Enum.TryParse(equipmentType, true, out EquipmentType type);
        if (!success)
        {
            throw new InvalidInputException("Not a valid type.");
        }

        List<Room> rooms = _hospital.RoomService.GetAll().ToList();
        ShowStockRooms(rooms);
        System.Console.Write("Chose a stock location to store: ");
        var location = ReadSanitizedLine();
        bool contains = rooms.Any(u => u.Location == location);
        if(!contains)
        {
            throw new InvalidInputException("Stock with that location does not exist!");
        }

        DateTime dateTime = DateTime.Now.AddMinutes(2);
        var request = new EquipmentRequest(emptyEquipments[number].Name, ammount, 
            type, dateTime, location);
        _hospital.EquipmentRequestService.Schedule(request);
        System.Console.Write("Successfully ordered equipment. Press anything to continue.");
        ReadSanitizedLine();
    }

    public void ShowEmptyEquipment(List<EquipmentAmount> emptyEquipments)
    {
        for(var i = 0; i < emptyEquipments.Count; i++)
        {
            System.Console.WriteLine(i + ". " + emptyEquipments[i].Name);
        }
    }

    public void ShowStockRooms(List<Room> rooms)
    {
        rooms.RemoveAll(room => room.Type != RoomType.STOCK);
        var roomUI = new RoomUI(_hospital, rooms);  
        System.Console.WriteLine("");
        roomUI.DisplayRooms();
    }

    public void MoveEquipment()
    {   
        System.Console.Clear();

        List<EquipmentBatch> missingEquipments = _hospital.EquipmentService.GetAll().ToList();
        missingEquipments.RemoveAll(u => u.Count > 5);
        missingEquipments.Sort(delegate(EquipmentBatch x, EquipmentBatch y){
            return x.Count.CompareTo(y.Count);
        });

        EquipmentTable(missingEquipments, "Missing");

        System.Console.Write("Chose location: ");
        var toLocation = ReadSanitizedLine();

        System.Console.Write("Chose equipment: ");
        var name = ReadSanitizedLine();
        bool contains = missingEquipments.Any(u => u.RoomLocation == toLocation && u.Name == name);
        if(!contains)
        {
            throw new InvalidInputException("Location or equipment inside the location does not exist!");
        }

        System.Console.Clear();

        List<EquipmentBatch> equipmentRoom = _hospital.EquipmentService.GetAll().ToList();
        equipmentRoom.RemoveAll(u => u.Name != name);
        equipmentRoom.RemoveAll(u => u.Count == 0);
        equipmentRoom.Sort(delegate(EquipmentBatch x, EquipmentBatch y){
            return x.Count.CompareTo(y.Count);
        });
        EquipmentTable(equipmentRoom, "To move");

        System.Console.Write("Chose location: ");
        var fromLocation = ReadSanitizedLine();

        System.Console.Write("Chose equipment: ");
        var count = ReadInt();
        bool succes = equipmentRoom.Any(u => u.RoomLocation == fromLocation && u.Count >= count);
        if(!succes)
        {
            throw new InvalidInputException("Location or equipment inside the location does not exist!");
        }

        DateTime endTime = DateTime.Now;

        var relocation = new EquipmentRelocation(name, count, EquipmentType.OPERATION, endTime, fromLocation, toLocation);
        _hospital.RelocationService.Schedule(relocation);
        System.Console.Write("Relocation scheduled successfully. Input anything to continue >> ");
        ReadSanitizedLine();
    }

    public void EquipmentTable(List<EquipmentBatch> equipments, string tableType)
    {   
        System.Console.WriteLine("~" + tableType + "~");
        System.Console.WriteLine("________________________________");
        System.Console.WriteLine(String.Format("| {0,-8} | {1,-9} | {2, -5} |","Location", "Equipment", "Count"));
        System.Console.WriteLine("|__________|___________|_______|");

        foreach(var equipment in equipments)
        {
            if (equipments.Count == 0)
            {
            System.Console.WriteLine(String.Format("|[{0,-8}]|[{1,-9}]|[{2, -5}]|", equipment.RoomLocation, equipment.Name, equipment.Count));
            }
            else
            {
            System.Console.WriteLine(String.Format("| {0,-8} | {1,-9} | {2, -5} |", equipment.RoomLocation, equipment.Name, equipment.Count));
            }
        }

        System.Console.WriteLine("|__________|___________|_______|");

    }
}

