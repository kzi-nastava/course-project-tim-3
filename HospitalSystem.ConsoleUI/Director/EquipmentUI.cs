using System.Text.RegularExpressions;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

public class EquipmentUI : ConsoleUI
{
    private List<EquipmentBatch> _loadedBatches;

    public EquipmentUI(Hospital hospital) : base(hospital)
    {
        _loadedBatches = _hospital.EquipmentService.GetAll().ToList();
    }

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("--- EQUIPMENT ---");
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
                    throw new QuitToMainMenuException("From StartManageEquipment.");
                }
                else if (choice == "x" || choice == "exit")
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    System.Console.WriteLine("Invalid input - please read the available commands.");
                    System.Console.Write("Input anything to continue >> ");
                    ReadSanitizedLine();
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Input anything to continue >> ");
                ReadSanitizedLine();
            }
            catch (InvalidTokenException e)
            {
                System.Console.Write(e.Message + " Input anything to continue >> ");
                ReadSanitizedLine();
            }
            catch (FormatException e)
            {
                System.Console.Write(e.Message + " Input anything to continue >> ");
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
        System.Console.Write("Select equip to move >> ");
        int index = ReadInt(0, _loadedBatches.Count - 1);
        var equipmentBatch = _loadedBatches[index];

        System.Console.Write("Select amount to move (minimum 1, available: " + equipmentBatch.Count + ") >> ");
        int amount = ReadInt(1, equipmentBatch.Count);

        System.Console.Write("Input date-time when it is done >> ");
        var rawDate = ReadSanitizedLine();
        var endTime = DateTime.Parse(rawDate);

        List<Room> rooms = _hospital.RoomService.GetAll().ToList();
        rooms.RemoveAll(room => room.Location == equipmentBatch.RoomLocation);
        var roomUI = new RoomUI(_hospital, rooms);  // TODO: this ugly...
        roomUI.DisplayRooms();
        System.Console.Write("Input room number >> ");
        var number = ReadInt(0, rooms.Count - 1);
        
        var relocation = new EquipmentRelocation(equipmentBatch.Name, amount, 
            equipmentBatch.Type, endTime, equipmentBatch.RoomLocation, rooms[number].Location);
        _hospital.RelocationService.Schedule(relocation);
        System.Console.Write("Relocation scheduled successfully. Input anything to continue >> ");
        ReadSanitizedLine();
    }

    private void Search()
    {
        System.Console.WriteLine("Example filters: min:3 max:100 type:checkup");
        System.Console.WriteLine("You can leave out any that you want to keep at previous value. Range is inclusive");
        System.Console.WriteLine("Available types: checkup, operation, furniture, hallway");

        System.Console.Write("Input your filters >> ");
        var filters = ReadSanitizedLine().Trim();
        var query = new EquipmentQuery(filters);

        System.Console.Write("Input your search term >> ");
        var search = ReadSanitizedLine();
        query.NameContains = new Regex(search);

        _loadedBatches = _hospital.EquipmentService.Search(query).ToList();
    }
}