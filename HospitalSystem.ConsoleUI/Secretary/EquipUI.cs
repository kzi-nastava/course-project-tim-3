using HospitalSystem.Core;
using HospitalSystem.Core.Rooms;
using HospitalSystem.Core.Equipment;
using HospitalSystem.Core.Equipment.Relocations;
using HospitalSystem.ConsoleUI.Director;

namespace HospitalSystem.ConsoleUI;

public class EquipUI : HospitalClientUI
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
                System.Console.WriteLine("  1. Procure equipment-(pe)");
                System.Console.WriteLine("  2. Move equipment-(me)");
                System.Console.WriteLine("  3. Quit-(q)");
                System.Console.WriteLine("  4. Exit-(x)");
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
                    return;
                }
                else if (choice == "exit" || choice == "x")
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    System.Console.Write("Unrecognized option. Intput anything to continue >> ");
                }
                
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Input anything to continue >> ");
            }
            catch (FormatException e)
            {
                System.Console.Write(e.Message + " Input anything to continue >> ");
            }
            ReadSanitizedLine();
        }
    }
    
    public void ProcureEquipment()
    {
        System.Console.Clear();

        List<EquipmentAmount> emptyEquipments = _hospital.EquipmentService.GetEmpty();

        System.Console.WriteLine("Out of stock: ");
        ShowEmptyEquipment(emptyEquipments);

        var number = EnterEquipmentNumber(emptyEquipments);
        var ammount = EnterEquipmentAmmount(emptyEquipments);
        EquipmentType type = EnterEquipmentType(emptyEquipments);

        List<Room> rooms = _hospital.RoomService.GetActive().ToList();
        ShowStockRooms(rooms);

        var location = EnterStockLocation(rooms);
        DateTime dateTime = DateTime.Now.AddDays(1);

        var order = new EquipmentOrder(emptyEquipments[number].Name, ammount, type, dateTime, location);
        _hospital.EquipmentOrderService.Schedule(order);
        System.Console.Write("Successfully ordered equipment. Press anything to continue.");
    }

    public int EnterEquipmentNumber(List<EquipmentAmount> emptyEquipments)
    {
        System.Console.Write("\nEnter the number of equipment: ");
        var number = ReadInt();
        if(number >= emptyEquipments.Count() || number < 0)
        {
            throw new InvalidInputException("That number does not exist");
        }
        return number;
    }

    public int EnterEquipmentAmmount(List<EquipmentAmount> emptyEquipments)
    {
        System.Console.Write("Enter the amount: ");
        var ammount = ReadInt();
        if(ammount <= 0)
        {
            throw new InvalidInputException("Ammount cant be negative or zero");
        }
        return ammount;
    }

    public EquipmentType EnterEquipmentType(List<EquipmentAmount> emptyEquipments)
    {
        System.Console.Write("Enter the equipment type (checkup, operation, furniture, hallway): ");
        var equipmentType = ReadSanitizedLine();
        
        bool success = Enum.TryParse(equipmentType, true, out EquipmentType type);
        if (!success)
        {
            throw new InvalidInputException("Not a valid type.");
        }
        return type;
    }

    public string EnterStockLocation( List<Room> rooms)
    {
        System.Console.Write("Chose a stock location to store: ");
        var location = ReadSanitizedLine();
        bool contains = rooms.Any(room => room.Location == location);
        if(!contains)
        {
            throw new InvalidInputException("Stock with that location does not exist!");
        }
        return location;
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

        List<EquipmentBatch> missingEquipments = _hospital.EquipmentService.GetLow();
        EquipmentTable(missingEquipments, "Missing");
        var toLocation = EnterLocation(missingEquipments);
        var name = EnterName(missingEquipments);

        System.Console.Clear();

        List<EquipmentBatch> equipmentRoom = _hospital.EquipmentService.GetExistingByName(name);
        equipmentRoom.RemoveAll(equipment => equipment.RoomLocation == toLocation && equipment.Name == name);
        EquipmentTable(equipmentRoom, "To move");
        var fromLocation = EnterLocation(missingEquipments);
        var count = EnterCount(missingEquipments);
        DateTime endTime = DateTime.Now;

        var relocation = new EquipmentRelocation(name, count, EquipmentType.OPERATION, endTime, fromLocation, toLocation);
        _hospital.RelocationService.Schedule(relocation);
        System.Console.Write("Successfully ordered equipment. Press anything to continue.");
    }

    public string EnterLocation(List<EquipmentBatch> equipments)
    {
        System.Console.Write("Chose location: ");
        var location = ReadSanitizedLine();
        bool success = equipments.Any(equipment => equipment.RoomLocation == location);
        if(!success)
        {
            throw new InvalidInputException("Location inside the table does not exist!");
        }
        return location;
    }

    public string EnterName(List<EquipmentBatch> equipments)
    {
        System.Console.Write("Choose equipment: ");
        var name = ReadSanitizedLine();
        bool success = equipments.Any(equipment => equipment.Name == name);
        if(!success)
        {
            throw new InvalidInputException("Equipment inside the table does not exist!");
        }
        return name;
    }

    public int EnterCount(List<EquipmentBatch> equipments)
    {
        System.Console.Write("Enter amount: ");
        var count = ReadInt();
        bool succes = equipments.Any(equipment => equipment.Count <= count);
        if(!succes)
        {
            throw new InvalidInputException("Invalid amount!");
        }
        return count;
    }

    public void EquipmentTable(List<EquipmentBatch> equipments, string header)
    {   
        System.Console.WriteLine("~" + header + "~");
        System.Console.WriteLine("________________________________");
        System.Console.WriteLine(String.Format("| {0,-8} | {1,-9} | {2, -5} |","Location", "Equipment", "Count"));
        System.Console.WriteLine("|__________|___________|_______|");

        foreach(var equipment in equipments)
        {
            List<Room> stocks = _hospital.RoomService.GetStocks().ToList();
            if(!stocks.Any(stock => stock.Location == equipment.RoomLocation)){
                if (equipment.Count == 0)
                {
                System.Console.WriteLine(String.Format("|[{0,-8}]|[{1,-9}]|[{2, -5}]|", equipment.RoomLocation, equipment.Name, equipment.Count));
                }
                else
                {
                System.Console.WriteLine(String.Format("| {0,-8} | {1,-9} | {2, -5} |", equipment.RoomLocation, equipment.Name, equipment.Count));
                }
            }
        }
        System.Console.WriteLine("|__________|___________|_______|");
    }
}

