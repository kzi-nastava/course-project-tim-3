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
                    continue;
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
    public void ProcureEquipment(){
        System.Console.Clear();

        List<EquipmentAmount> emptyEquipments = _hospital.EquipmentService.GetEmpty();
        System.Console.WriteLine("Out of stock: ");
        showEmptyEquipment(emptyEquipments);

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
        showStockRooms(rooms);
        System.Console.Write("Chose a stock location to store: ");
        var location = ReadSanitizedLine();
        bool contains = rooms.Any(u => u.Location == location);
        if(!contains){
            throw new InvalidInputException("Stock with that location does not exist!");
        }

        DateTime dateTime = DateTime.Now.AddMinutes(2);
        var request = new EquipmentRequest(emptyEquipments[number].Name, ammount, 
            type, dateTime, location);
        _hospital.EquipmentRequestService.Schedule(request);
        System.Console.Write("Successfully ordered equipment. Press anything to continue.");
        ReadSanitizedLine();


    }

    public void showEmptyEquipment(List<EquipmentAmount> emptyEquipments)
    {
        for(var i = 0; i < emptyEquipments.Count; i++){
            System.Console.WriteLine(i + ". " + emptyEquipments[i].Name);
        }
    }

    public void showStockRooms(List<Room> rooms)
    {
        rooms.RemoveAll(room => room.Type != RoomType.STOCK);
        var roomUI = new RoomUI(_hospital, rooms);  
        System.Console.WriteLine("");
        roomUI.DisplayRooms();
    }
}

