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
                        [...]
                    ");
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
            while (true)
            {
                System.Console.Clear();
                System.Console.WriteLine(@"
                --- MANAGE ROOMS ---
                INPUT OPTION:
                    [add room|add|ar|a] Add a room
                    [quit|q] Quit to main menu
                    [exit|x] Exit program
                    [...]
                ");
                System.Console.Write(">> ");
                var choice = ReadSanitizedLine();
                try
                {
                    if (choice == "a" || choice == "ar" || choice == "add" || choice == "add rooms")
                    {
                        System.Console.Write("ENTER ROOM LOCATION >> ");
                        var location = ReadSanitizedLine();
                        System.Console.Write("ENTER ROOM NAME >> ");
                        var name = ReadSanitizedLine();
                        System.Console.Write("ENTER ROOM TYPE [rest|operation|examination|other] >> ");
                        var rawType = ReadSanitizedLine();
                        Enum.TryParse(rawType, true, out Room.RoomType type);
                        var newRoom = new Room(location, name, type);
                        _hospital.RoomRepo.AddRoom(newRoom);
                        System.Console.Write("SUCCESSFULLY ADDED ROOM. INPUT ANYTHING TO CONTINUE >> ");
                        ReadSanitizedLine();
                    }
                    else if (choice == "q" || choice == "quit")
                        throw new QuitToMainMenuException("From StartManageRooms");
                    else if (choice == "x" || choice == "exit")
                        System.Environment.Exit(0);
                }
                catch (ArgumentException)
                {
                    System.Console.Write("NOT A VALID TYPE! INPUT ANYTHING TO CONTINUE >> ");
                    ReadSanitizedLine();
                }
            }

        }
    }
}