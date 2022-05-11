namespace Hospital;

public class RoomUI : ConsoleUI
{
    public RoomUI(Hospital hospital) : base(hospital)
    {

    }

    public override void Start()
    {
        while (true)
        {
            List<Room> rooms = _hospital.RoomRepo.GetAll().ToList();
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
                    Add();
                }
                else if (choice == "u" || choice == "ur" || choice == "update" || choice == "update room")
                {
                    Update(rooms);
                }
                else if (choice == "delete room" || choice == "delete" || choice == "dr" || choice == "d")
                {
                    Delete(rooms);
                }
                else if (choice == "q" || choice == "quit")
                {
                    throw new QuitToMainMenuException("From StartManageRooms");
                }
                else if (choice == "x" || choice == "exit")
                {
                    System.Environment.Exit(0);
                }
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

    // TODO: this static but not in EquipUI??
    public static void DisplayRooms(List<Room> rooms)
    {
        System.Console.WriteLine("No. | Location | Name | Type");
        // TODO: paginate and make prettier
        for (int i = 0; i < rooms.Count; i++)
        {
            var room = rooms[i];
            System.Console.WriteLine(i + " | " + room.Location + " | " + room.Name + " | " + room.Type);
        }
    }

    private void Update(List<Room> rooms)
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

        _hospital.RoomRepo.Replace(room);
        System.Console.Write("SUCCESSFULLY UPDATED ROOM. INPUT ANYTHING TO CONTINUE >> ");
    }

    private void Add()
    {
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
        _hospital.RoomRepo.Add(newRoom);
        System.Console.Write("SUCCESSFULLY ADDED ROOM. INPUT ANYTHING TO CONTINUE >> ");
    }

    private void Delete(List<Room> rooms)
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
        _hospital.RoomRepo.Delete(rooms[number].Id);
        System.Console.Write("SUCCESSFULLY DELETED ROOM. INPUT ANYTHING TO CONTINUE >> ");
    }
}