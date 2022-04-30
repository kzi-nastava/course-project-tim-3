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
                        [exit|x] Exit program
                    ");  // TODO: add the rest of the features
                    System.Console.Write(">> ");
                    var choice = ReadSanitizedLine();
                    if (choice == "mr" || choice == "manage rooms")
                        StartManageRooms();
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
                        bool success = Enum.TryParse(rawType, true, out Room.RoomType type);
                        if (!success)
                            throw new InvalidInputException("NOT A VALID TYPE!");

                        var newRoom = new Room(location, name, type);
                        _hospital.RoomRepo.AddRoom(newRoom);
                        System.Console.Write("SUCCESSFULLY ADDED ROOM. INPUT ANYTHING TO CONTINUE >> ");
                        ReadSanitizedLine();
                    }
                    else if (choice == "u" || choice == "ur" || choice == "update" || choice == "update room")
                    {
                        // todo: move to a special function number bounds input
                        System.Console.Write("INPUT NUMBER >> ");
                        var rawNumber = ReadSanitizedLine();
                        bool success = Int32.TryParse(rawNumber, out int number);
                        if (number < 0 || number >= rooms.Count)
                            throw new InvalidInputException("NUMBER OUT OF BOUNDS!");
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
                        Room.RoomType type;
                        if (rawType != "")
                        {
                            success = Enum.TryParse(rawType, true, out type);
                            if (!success)
                                throw new InvalidInputException("NOT A VALID TYPE!");
                            room.Type = type;
                        }

                        _hospital.RoomRepo.UpdateRoom(room);
                        System.Console.Write("SUCCESSFULLY UPDATED ROOM. INPUT ANYTHING TO CONTINUE >> ");
                        ReadSanitizedLine();
                    }
                    else if (choice == "delete room" || choice == "delete" || choice == "dr" || choice == "d")
                    {
                        System.Console.Write("INPUT NUMBER >> ");
                        var rawNumber = ReadSanitizedLine();
                        bool success = Int32.TryParse(rawNumber, out int number);
                        if (number < 0 || number >= rooms.Count)
                            throw new InvalidInputException("NUMBER OUT OF BOUNDS!");
                        
                        _hospital.RoomRepo.DeleteRoom(rooms[number].Id);
                        System.Console.Write("SUCCESSFULLY DELETED ROOM. INPUT ANYTHING TO CONTINUE >> ");
                        ReadSanitizedLine();
                    }
                    else if (choice == "q" || choice == "quit")
                        throw new QuitToMainMenuException("From StartManageRooms");
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
    }
}