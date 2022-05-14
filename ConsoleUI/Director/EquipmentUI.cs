using System.Text.RegularExpressions;

namespace HospitalSystem;

public class EquipmentUI : ConsoleUI
{
    private List<EquipmentBatch> _loadedBatches;

    public EquipmentUI(Hospital hospital) : base(hospital)
    {
        _loadedBatches = _hospital.EquipmentRepo.GetAll().ToList();
    }

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("--- EQUIPMENTS ---");
            DisplayBatches(_loadedBatches);
            System.Console.WriteLine(@"
            INPUT OPTION:
                [search equipment|search|se] Search equipment batches
                [move equipment|move|me]
                [quit|q] Quit to main menu
                [exit|x] Exit program
            ");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                // todo: unhardcode choices so they match menu display always
                if (choice == "se" || choice == "search" || choice == "search equipment")
                {
                    Search();
                }
                else if (choice == "me" || choice == "move" || choice == "move equipment")
                {
                    Move();
                }
                else if (choice == "q" || choice == "quit")
                {
                    throw new QuitToMainMenuException("From StartManageEquipments");
                }
                else if (choice == "x" || choice == "exit")
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    System.Console.WriteLine("INVALID INPUT - READ THE AVAILABLE COMMANDS!");
                    System.Console.Write("INPUT ANYTHING TO CONTINUE >> ");
                    ReadSanitizedLine();
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " INPUT ANYTHING TO CONTINUE >> ");
                ReadSanitizedLine();
            }
            catch (FormatException e)
            {
                System.Console.Write(e.Message + " INPUT ANYTHING TO CONTINUE >> ");
                ReadSanitizedLine();
            }
        }
    }

    public void DisplayBatches(List<EquipmentBatch> batches)
    {
        System.Console.WriteLine("No. | Room Location | Type | Name | Count");
        // TODO: paginate and make prettier
        for (int i = 0; i < batches.Count; i++)
        {
            var batch = batches[i];
            // TODO: exception if room is null
            System.Console.WriteLine(i + " | " + batch.RoomLocation + " | " + batch.Type + 
                                        " | " + batch.Name + " | " + batch.Count);
        }
    }

    private void Move()
    {
        System.Console.Write("SELECT EQUIP TO MOVE >> ");
        int index = ReadInt(0, _loadedBatches.Count - 1);
        var equipmentBatch = _loadedBatches[index];

        System.Console.Write("SELECT AMOUNT TO MOVE (MINIMUM 1. AVAILABLE: " + equipmentBatch.Count + ") >> ");
        int amount = ReadInt(1, equipmentBatch.Count);

        System.Console.Write("INPUT DATE-TIME WHEN IT IS DONE >> ");
        var rawDate = ReadSanitizedLine();
        var endTime = DateTime.Parse(rawDate);

        List<Room> rooms = _hospital.RoomRepo.GetAll().ToList();
        rooms.RemoveAll(room => room.Location == equipmentBatch.RoomLocation);
        var roomUI = new RoomUI(_hospital, rooms);  // TODO: this ugly...
        roomUI.DisplayRooms();
        System.Console.Write("INPUT ROOM NUMBER >> ");
        var number = ReadInt(0, rooms.Count - 1);
        
        var relocation = new EquipmentRelocation(equipmentBatch.Name, amount, 
            equipmentBatch.Type, endTime, equipmentBatch.RoomLocation, rooms[number].Location);
        _hospital.RelocationRepo.Add(relocation);
        _hospital.RelocationRepo.Schedule(relocation);
        System.Console.Write("RELOCATION SCHEDULED SUCCESSFULLY. INPUT ANYTHING TO CONTINUE >> ");
        ReadSanitizedLine();
    }

    private void Search()
    {
        System.Console.WriteLine("Example filters: min:3 max:100 type:checkup");
        System.Console.WriteLine("You can leave out any that you want to keep at any value. Range is inclusive");
        System.Console.WriteLine("Available types: checkup, operation, furniture, hallway");

        System.Console.Write("INPUT YOUR FILTERS >> ");
        var filters = ReadSanitizedLine().Trim();
        var query = new EquipmentQuery(filters);

        System.Console.Write("INPUT YOUR SEARCH TERM >> ");
        var search = ReadSanitizedLine();
        query.NameContains = new Regex(search);

        _loadedBatches = _hospital.EquipmentRepo.Search(query).ToList();
    }
}