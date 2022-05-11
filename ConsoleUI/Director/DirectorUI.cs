using System.Text.RegularExpressions;
using MongoDB.Bson;

namespace Hospital;

public class DirectorUI : ConsoleUI
{
    public DirectorUI(Hospital _hosptial) : base(_hosptial)
    {
        
    }

    public override void Start()
    {
        while (true)
        {
            try
            {
                System.Console.Clear();
                System.Console.WriteLine(@"
                INPUT OPTION:
                    [manage rooms|mr] Manage rooms and stockrooms
                    [manage equipment|me] Manage equipment
                    [log out|lo] Log out
                    [exit|x] Exit program
                ");  // TODO: add the rest of the features
                System.Console.Write(">> ");
                var choice = ReadSanitizedLine();
                if (choice == "mr" || choice == "manage rooms")
                    StartManageRooms();
                else if (choice == "me" || choice == "manage equipment")
                    StartManageEquipment();
                else if (choice == "lo" || choice == "log out")
                    return;
                else if (choice == "x" || choice == "exit")
                    System.Environment.Exit(0);
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

    public void StartManageRooms()
    {
        while (true)
        {
            List<Room> rooms = _hospital.RoomRepo.GetQueryableRooms().ToList();
            System.Console.Clear();
            System.Console.WriteLine("--- ROOMS ---");
            DisplayRooms(rooms);
            System.Console.WriteLine(@"
            INPUT OPTION:
                [add room|add|ar|a] Add a room
                [update room|update|ur|u] Update a room
                [delete room|delete|dr|d] Delete a room
                [quit|q] Quit to main menu
                [exit|x] Exit program
            ");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                // todo: unhardcode choices so they match menu display always
                if (choice == "a" || choice == "ar" || choice == "add" || choice == "add rooms")
                {
                    // todo: move to function everything...
                    System.Console.Write("ENTER ROOM LOCATION >> ");
                    var location = ReadSanitizedLine();
                    if (location == "")
                        throw new InvalidInputException("INVALID LOCATION!");

                    System.Console.Write("ENTER ROOM NAME >> ");
                    var name = ReadSanitizedLine();
                    if (name == "")
                        throw new InvalidInputException("INVALID NAME!");

                    System.Console.Write("ENTER ROOM TYPE [rest|operation|checkup|other] >> ");
                    var rawType = ReadSanitizedLine();
                    bool success = Enum.TryParse(rawType, true, out RoomType type);
                    if (!success || type == RoomType.STOCK)
                        throw new InvalidInputException("NOT A VALID TYPE!");

                    var newRoom = new Room(location, name, type);
                    _hospital.RoomRepo.AddRoom(newRoom);
                    System.Console.Write("SUCCESSFULLY ADDED ROOM. INPUT ANYTHING TO CONTINUE >> ");
                }
                else if (choice == "u" || choice == "ur" || choice == "update" || choice == "update room")
                {
                    System.Console.Write("INPUT NUMBER >> ");
                    var number = ReadInt(0, rooms.Count - 1);
                    var room = rooms[number];

                    System.Console.WriteLine("INPUT NOTHING TO KEEP AS IS");

                    System.Console.Write("ENTER ROOM LOCATION >> ");
                    var location = ReadSanitizedLine();
                    if (location != "")
                        room.Location = location;

                    System.Console.Write("ENTER ROOM NAME >> ");
                    var name = ReadSanitizedLine();
                    if (name != "")
                        room.Name = name;

                    System.Console.Write("ENTER ROOM TYPE [rest|operation|checkup|other] >> ");
                    var rawType = ReadSanitizedLine();
                    RoomType type;
                    if (rawType != "")
                    {
                        var success = Enum.TryParse(rawType, true, out type);
                        if (!success || type == RoomType.STOCK)
                            throw new InvalidInputException("NOT A VALID TYPE!");
                        room.Type = type;
                    }

                    _hospital.RoomRepo.UpdateRoom(room);
                    System.Console.Write("SUCCESSFULLY UPDATED ROOM. INPUT ANYTHING TO CONTINUE >> ");
                }
                else if (choice == "delete room" || choice == "delete" || choice == "dr" || choice == "d")
                {
                    System.Console.Write("INPUT NUMBER >> ");
                    var number = ReadInt(0, rooms.Count - 1);
                    if (_hospital.EquipmentRepo.GetAllInRoom(rooms[number]).Any())
                    {
                        // TODO: make into a moving equipment submenu
                        System.Console.Write("THIS ROOM HAS EQUIPMENT IN IT. THIS OPERATION WILL DELETE IT ALL. ARE YOU SURE? [y/N] >> ");
                        var answer = ReadSanitizedLine();
                        if (answer != "y")
                            throw new AbortException("NOT A YES. ABORTING.");
                        _hospital.EquipmentRepo.DeleteInRoom(rooms[number]);
                    }
                    _hospital.RoomRepo.DeleteRoom(rooms[number].Id);
                    System.Console.Write("SUCCESSFULLY DELETED ROOM. INPUT ANYTHING TO CONTINUE >> ");
                }
                else if (choice == "q" || choice == "quit")
                    throw new QuitToMainMenuException("From StartManageRooms");
                else if (choice == "x" || choice == "exit")
                    System.Environment.Exit(0);
                else
                {
                    System.Console.WriteLine("INVALID INPUT - READ THE AVAILABLE COMMANDS!");
                    System.Console.Write("INPUT ANYTHING TO CONTINUE >> ");
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " INPUT ANYTHING TO CONTINUE >> ");
            }
            catch (AbortException e)
            {  // TODO: make hierarchy extension same with above
                System.Console.Write(e.Message + " INPUT ANYTHING TO CONTINUE >> ");
            }
            ReadSanitizedLine();
        }
    }

    public void StartManageEquipment()
    {
        // TODO: load schedules
        List<EquipmentBatch> equipmentBatches = _hospital.EquipmentRepo.GetAll().ToList();
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("--- EQUIPMENTS ---");
            DisplayEquipmentBatches(equipmentBatches);
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
                    System.Console.WriteLine("Example filters: min:3 max:100 type:checkup");
                    System.Console.WriteLine("You can leave out any that you want to keep at any value. Range is inclusive");
                    System.Console.WriteLine("Available types: checkup, operation, furniture, hallway");

                    System.Console.Write("INPUT YOUR FILTERS >> ");
                    var filters = ReadSanitizedLine().Trim();
                    var query = new EquipmentQuery(filters);

                    System.Console.Write("INPUT YOUR SEARCH TERM >> ");
                    var search = ReadSanitizedLine();
                    query.NameContains = new Regex(search);

                    equipmentBatches = SearchEquipmentBatches(query).ToList();
                }
                else if (choice == "me" || choice == "move" || choice == "move equipment")
                {
                    // TODO: functionalize
                    System.Console.Write("SELECT EQUIP TO MOVE >> ");
                    int index = ReadInt(0, equipmentBatches.Count - 1);
                    var equipmentBatch = equipmentBatches[index];

                    System.Console.Write("SELECT AMOUNT TO MOVE (MINIMUM 1. AVAILABLE: " + equipmentBatch.Count + ") >> ");
                    int amount = ReadInt(1, equipmentBatch.Count);

                    System.Console.Write("INPUT DATE-TIME WHEN IT IS DONE >> ");
                    var rawDate = ReadSanitizedLine();
                    var whenDone = DateTime.Parse(rawDate);

                    List<Room> rooms = _hospital.RoomRepo.GetQueryableRooms().ToList();
                    DisplayRooms(rooms);
                    System.Console.Write("INPUT ROOM NUMBER >> ");
                    var number = ReadInt(0, rooms.Count - 1);
                    
                    var relocation = new EquipmentRelocation(equipmentBatch.Name, amount, 
                        equipmentBatch.Type, whenDone, (ObjectId) equipmentBatch.Room.Id, rooms[number].Id);
                    _hospital.RelocationRepo.Add(relocation);
                    _hospital.RelocationRepo.Schedule(relocation);
                    System.Console.Write("RELOCATION SCHEDULED SUCCESSFULLY. INPUT ANYTHING TO CONTINUE >> ");
                    ReadSanitizedLine();
                }
                else if (choice == "q" || choice == "quit")
                    throw new QuitToMainMenuException("From StartManageEquipments");
                else if (choice == "x" || choice == "exit")
                    System.Environment.Exit(0);
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

    private IQueryable<EquipmentBatch> SearchEquipmentBatches(EquipmentQuery query)  // TODO: probably have to move this
    {
        var equipmentBatches = _hospital.EquipmentRepo.GetAll();
        var matches = 
            from equipmentBatch in equipmentBatches
            where (query.MinCount == null || query.MinCount <= equipmentBatch.Count)
                && (query.MaxCount == null || query.MaxCount >= equipmentBatch.Count)
                && (query.Type == null || query.Type == equipmentBatch.Type)
                && (query.NameContains == null || query.NameContains.IsMatch(equipmentBatch.Name))
            select equipmentBatch;
        return matches;
    }

    public void DisplayRooms(List<Room> rooms)
    {
        System.Console.WriteLine("No. | Location | Name | Type");
        // TODO: paginate and make prettier
        for (int i = 0; i < rooms.Count; i++)
        {
            var room = rooms[i];
            System.Console.WriteLine(i + " | " + room.Location + " | " + room.Name + " | " + room.Type);
        }
    }

    public void DisplayEquipmentBatches(List<EquipmentBatch> equipmentBatches)
    {
        System.Console.WriteLine("No. | Room Location | Type | Name | Count");
        // TODO: paginate and make prettier
        for (int i = 0; i < equipmentBatches.Count; i++)
        {
            var equipmentBatch = equipmentBatches[i];
            var room = _hospital.RoomRepo.GetRoom((ObjectId) equipmentBatch.Room.Id);
            // TODO: exception if room is null
            System.Console.WriteLine(i + " | " + room?.Location + " | " + equipmentBatch.Type + 
                                        " | " + equipmentBatch.Name + " | " + equipmentBatch.Count);
        }
    }

    private struct EquipmentQuery
    {
        public int? MinCount { get; set; }
        public int? MaxCount { get; set; }
        public EquipmentType? Type { get; set; }
        public Regex? NameContains { get; set; }

        public EquipmentQuery(string query)
        {
            // TODO: make it so repeated same will throw error
            MinCount = null;
            MaxCount = null;
            Type = null;
            NameContains = null;
            if (query == "")
                return;
            var tokens = query.Split();
            foreach (var token in tokens)
            {
                if (token.StartsWith("min:"))
                {
                    bool success = Int32.TryParse(token.Substring(4), out int number);
                    if (!success)
                        throw new InvalidInputException("GIVEN MIN IS NOT A NUMBER.");
                    MinCount = number;
                } 
                else if (token.StartsWith("max:"))
                {
                    bool success = Int32.TryParse(token.Substring(4), out int number);
                    if (!success)
                        throw new InvalidInputException("GIVEN MAX IS NOT A NUMBER.");
                    MaxCount = number;
                }
                else if (token.StartsWith("type:"))
                {
                    EquipmentType type;
                    var success = Enum.TryParse(token.Substring(5), true, out type);
                    if (!success)
                        throw new InvalidInputException("NOT A VALID TYPE!");
                    Type = type;
                }
                else
                {
                    throw new InvalidInputException("UNRECOGNIZED TOKEN: " + token);
                }
            }
        }
    }
}