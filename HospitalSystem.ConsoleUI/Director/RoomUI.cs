using HospitalSystem.Core;
using HospitalSystem.Core.Renovations;

namespace HospitalSystem.ConsoleUI.Director;

public class RoomUI : HospitalClientUI
{
    protected List<Room> _loadedRooms;

    public RoomUI(Hospital hospital) : base(hospital)
    {
        _loadedRooms = _hospital.RoomService.GetActive().ToList();
    }

    public RoomUI(Hospital hospital, List<Room> loadedRooms) : base(hospital)
    {
        _loadedRooms = loadedRooms;
    }

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("--- ROOMS ---");
            DisplayRooms();
            System.Console.WriteLine(@"
            INPUT OPTION:
                [add room|add|ar|a] Add a room
                [update room|update|ur|u] Update a room
                [delete room|delete|dr|d] Delete a room
                [renovate room|renovate|rr|r] Renovate a room
                [split renovation|split|sr|s] Renovate a room by splitting it in two
                [merge renovation|merge|mr|m] Renovate a rooms by merging two of them
                [quit|q] Quit to main menu
                [exit|x] Exit program
            ");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            var renoUI = new RenovationUI(_hospital, _loadedRooms);
            try
            {
                // todo: unhardcode choices so they match menu display always
                if (choice == "a" || choice == "ar" || choice == "add" || choice == "add rooms")
                {
                    Insert();
                }
                else if (choice == "u" || choice == "ur" || choice == "update" || choice == "update room")
                {
                    Update();
                }
                else if (choice == "delete room" || choice == "delete" || choice == "dr" || choice == "d")
                {
                    Delete();
                }
                else if (choice == "renovate room" || choice == "renovate" || choice == "rr" || choice == "r")
                {
                    renoUI.ScheduleSimple();
                }
                else if (choice == "s" || choice == "sr" || choice == "split" || choice == "split renovation")
                {
                    renoUI.ScheduleSplit();
                }
                else if (choice == "m" || choice == "mr" || choice == "merge" || choice == "merge renovation")
                {
                    renoUI.ScheduleMerge();
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
                    System.Console.WriteLine("Invalid input: please read the available commands.");
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message);
            }
            catch (AbortException e)
            {  // TODO: make hierarchy extension same with above
                System.Console.Write(e.Message);
            }
            catch (FormatException e)
            {
                System.Console.Write(e.Message);
            }
            catch (RenovationException e)
            {
                System.Console.Write(e.Message);
            }
            catch (ArgumentException e)
            {
                System.Console.Write(e.Message);
            }
            _loadedRooms = _hospital.RoomService.GetActive().ToList();
            System.Console.Write("\nInput anything to continue >> ");
            ReadSanitizedLine();
        }
    }

    public void DisplayRooms()
    {
        System.Console.WriteLine("No. | Location | Name | Type");
        // TODO: paginate and make prettier
        for (int i = 0; i < _loadedRooms.Count; i++)
        {
            var room = _loadedRooms[i];
            System.Console.WriteLine(i + " | " + room.Location + " | " + room.Name + " | " + room.Type);
        }
    }

    private void Update()
    {
        System.Console.Write("Input number >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);
        var room = _loadedRooms[number];

        System.Console.WriteLine("Input nothing to keep as is");

        System.Console.Write("Enter room location >> ");
        room.Location = ReadUpdate(room.Location);

        System.Console.Write("Enter room name >> ");
        room.Name = ReadUpdate(room.Name);

        System.Console.Write("Enter room type [rest|operation|checkup|other] >> ");
        room.Type = InputRoomType(update: true, defaultType: room.Type);

        _hospital.RoomService.Replace(room);
        System.Console.Write("Successfully updated room.");
    }

    private void Insert()
    {
        var newRoom = InputRoom();
        _hospital.RoomService.Insert(newRoom);
        System.Console.Write("Successfully added room.");
    }

    protected Room InputRoom()
    {
        System.Console.Write("Enter room location >> ");
        var location = ReadNotEmpty("Invalid location.");

        if (_hospital.RoomService.DoesExist(location))  // TODO: move this check
            throw new InvalidInputException("Room with that location already exists.");

        System.Console.Write("Enter room name >> ");
        var name = ReadNotEmpty("Invalid name.");

        System.Console.Write("Enter room type [rest|operation|checkup|other] >> ");
        var type = InputRoomType();

        return new Room(location, name, type);
    }

    private RoomType InputRoomType(bool update = false, RoomType defaultType = RoomType.OTHER)
    {
        var rawType = ReadSanitizedLine();
        if (update && rawType == "")
            return defaultType;
        bool success = Enum.TryParse(rawType, true, out RoomType type);
        if (!success || type == RoomType.STOCK)
            throw new InvalidInputException("Not a valid type.");
        return type;
    }

    private void Delete()
    {
        System.Console.Write("Input number >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);
        if (_hospital.EquipmentService.GetAllIn(_loadedRooms[number].Location).Any())
        {
            // TODO: make into a moving equipment submenu
            System.Console.Write("This room has equipment in it. This operation will delete it all. Are you sure? [y/N] >> ");
            if (!ReadYes())
                throw new AbortException("Not a yes. Aborting.");
            _hospital.EquipmentService.DeleteAllInRoom(_loadedRooms[number]);
        }
        _hospital.RoomService.Delete(_loadedRooms[number].Location);
        System.Console.Write("Successfully deleted room.");
    }
}