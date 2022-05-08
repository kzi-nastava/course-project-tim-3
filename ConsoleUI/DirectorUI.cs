using System.Text.RegularExpressions;
using MongoDB.Bson;

namespace Hospital
{
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
                        [exit|x] Exit program
                    ");  // TODO: add the rest of the features
                    System.Console.Write(">> ");
                    var choice = ReadSanitizedLine();
                    if (choice == "mr" || choice == "manage rooms")
                        StartManageRooms();
                    else if (choice == "me" || choice == "manage equipment")
                        StartManageEquipment();
                    else if (choice == "x" || choice == "exit")
                        System.Environment.Exit(0);
                }
                catch (QuitToMainMenuException)
                {

                }
            }
        }

        public void StartManageRooms()
        {
            List<Room> rooms = _hospital.RoomRepo.GetQueryableRooms().ToList();
            while (true)
            {
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
                    [...]
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

                        System.Console.Write("ENTER ROOM TYPE [rest|operation|examination|other] >> ");
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

                        System.Console.Write("ENTER ROOM TYPE [rest|operation|examination|other] >> ");
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
                ReadSanitizedLine();
            }
        }

        public void StartManageEquipment()
        {
            List<Equipment> equipments = _hospital.EquipmentRepo.GetQueryableEquipments().ToList();
            while (true)
            {
                System.Console.Clear();
                System.Console.WriteLine("--- EQUIPMENTS ---");
                DisplayEquipment(equipments);
                System.Console.WriteLine(@"
                INPUT OPTION:
                    [search|se] Search equipments
                    [quit|q] Quit to main menu
                    [exit|x] Exit program
                    [...]
                ");
                System.Console.Write(">> ");
                var choice = ReadSanitizedLine();
                try
                {
                    // todo: unhardcode choices so they match menu display always
                    if (choice == "se" || choice == "search")
                    {
                        equipments = _hospital.EquipmentRepo.GetQueryableEquipments().ToList();
                        System.Console.WriteLine("Example filters: min:3 max:100 type:checkup");
                        System.Console.WriteLine("You can leave out any that you want to keep at any value. Range is inclusive");
                        System.Console.WriteLine("Available types: checkup, operation, furniture, hallway");
                        System.Console.Write("INPUT YOUR FILTERS >> ");
                        var filters = ReadSanitizedLine().Trim();
                        var query = new Query(filters);

                        System.Console.Write("INPUT YOUR SEARCH TERM >> ");
                        var search = ReadSanitizedLine();
                        query.NameContains = new Regex(search);
                        var matches = 
                            from equipment in equipments
                            where (query.MinCount is null || query.MinCount <= equipment.Count)
                                && (query.MaxCount is null || query.MaxCount >= equipment.Count)
                                && (query.Type is null || query.Type == equipment.Type)
                                && (query.NameContains is null || query.NameContains.IsMatch(equipment.Name))
                            select equipment;
                        equipments = matches.ToList();
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
            }
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

        public void DisplayEquipment(List<Equipment> equipments)
        {
            System.Console.WriteLine("No. | Room Location | Type | Name | Count");
            // TODO: paginate and make prettier
            for (int i = 0; i < equipments.Count; i++)
            {
                var equipment = equipments[i];
                var room = _hospital.RoomRepo.GetRoom((ObjectId) equipment.Room.Id);
                // TODO: exception if room is null
                System.Console.WriteLine(i + " | " + room?.Location + " | " + equipment.Type + 
                                         " | " + equipment.Name + " | " + equipment.Count);
            }
        }

        private struct Query
        {
            public int? MinCount { get; set; }
            public int? MaxCount { get; set; }
            public EquipmentType? Type { get; set; }
            public Regex? NameContains { get; set; }

            public Query(string query)
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
                        int number;
                        bool success = Int32.TryParse(token.Substring(4), out number);
                        if (!success)
                            throw new InvalidInputException("GIVEN MIN IS NOT A NUMBER.");
                        MinCount = number;
                    } 
                    else if (token.StartsWith("max:"))
                    {
                        int number;
                        bool success = Int32.TryParse(token.Substring(4), out number);
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
}