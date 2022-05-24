using HospitalSystem.Utils;

namespace HospitalSystem;

public class RoomUI : ConsoleUI
{
    private List<Room> _loadedRooms;

    public RoomUI(Hospital hospital) : base(hospital)
    {
        _loadedRooms = _hospital.RoomService.GetAll().ToList();
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
            try
            {
                // todo: unhardcode choices so they match menu display always
                if (choice == "a" || choice == "ar" || choice == "add" || choice == "add rooms")
                {
                    Add();
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
                    DoSimpleRenovation();
                }
                else if (choice == "s" || choice == "sr" || choice == "split" || choice == "split renovation")
                {
                    DoSplitRenovation();
                }
                else if (choice == "m" || choice == "mr" || choice == "merge" || choice == "merge renovation")
                {
                    DoMergeRenovation();
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
            _loadedRooms = _hospital.RoomService.GetAll().ToList();
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
        var location = ReadSanitizedLine();
        if (location != "")
            room.Location = location;

        System.Console.Write("Enter room name >> ");
        var name = ReadSanitizedLine();
        if (name != "")
            room.Name = name;

        System.Console.Write("Enter room type [rest|operation|checkup|other] >> ");
        var rawType = ReadSanitizedLine();
        RoomType type;
        if (rawType != "")
        {
            var success = Enum.TryParse(rawType, true, out type);
            if (!success || type == RoomType.STOCK)
                throw new InvalidInputException("Not a valid type.");
            room.Type = type;
        }

        _hospital.RoomService.Replace(room);
        System.Console.Write("Successfully updated room. Input anything to continue >> ");
    }

    private void Add()
    {
        var newRoom = InputRoom();
        _hospital.RoomService.Insert(newRoom);
        System.Console.Write("Successfully added room. Input anything to continue >> ");
    }

    private Room InputRoom()
    {
        System.Console.Write("Enter room location >> ");
        var location = ReadSanitizedLine();
        if (location == "")
            throw new InvalidInputException("Invalid location.");
        if (_hospital.RoomService.DoesExist(location))
            throw new InvalidInputException("Room with that location already exists.");

        System.Console.Write("Enter room name >> ");
        var name = ReadSanitizedLine();
        if (name == "")
            throw new InvalidInputException("Invalid name.");

        System.Console.Write("Enter room type [rest|operation|checkup|other] >> ");
        var rawType = ReadSanitizedLine();
        bool success = Enum.TryParse(rawType, true, out RoomType type);
        if (!success || type == RoomType.STOCK)
            throw new InvalidInputException("Not a valid type.");

        return new Room(location, name, type);
    }

    private void Delete()
    {
        System.Console.Write("Input number >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);
        if (_hospital.EquipmentService.GetAllIn(_loadedRooms[number].Location).Any())
        {
            // TODO: make into a moving equipment submenu
            System.Console.Write("This room has equipment in it. This operation will delete it all. Are you sure? [y/N] >> ");
            var answer = ReadSanitizedLine();
            if (answer != "y")
                throw new AbortException("Not a yes. Aborting.");
            _hospital.EquipmentService.DeleteAllInRoom(_loadedRooms[number]);
        }
        _hospital.RoomService.Delete(_loadedRooms[number].Location);
        System.Console.Write("Successfully deleted room. Input anything to continue >> ");
    }

    private void DoSimpleRenovation()
    {
        System.Console.WriteLine("Warning! Doing this will make any equipment inside inaccessible during renovation. ");
        System.Console.WriteLine("Move it first if you so desire");
        System.Console.Write("Input number >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);

        var range = InputDateRange();

        if (!_hospital.AppointmentRepo.IsRoomAvailableForRenovation(_loadedRooms[number].Location, range.Starts))
        {
            throw new InvalidInputException("That room has appointments scheduled, can't renovate");
        }

        var renovation = new SimpleRenovation(_loadedRooms[number].Location, range);
        _hospital.SimpleRenovationService.Schedule(renovation);
        System.Console.Write("Successfully scheduled simple renovation. Input anything to continue >>  ");
    }

    private void DoSplitRenovation()
    {
        System.Console.WriteLine("Warning! Doing this will make any equipment inside inaccessible during renovation");
        System.Console.WriteLine("This will move all equipment present at the beginning of the renovation into the first room");
        System.Console.WriteLine("Move it first if you so desire");
        System.Console.Write("Input number to split >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);
        var originalRoom = _loadedRooms[number];

        var range = InputDateRange();

        if (!_hospital.AppointmentRepo.IsRoomAvailableForRenovation(originalRoom.Location, range.Starts))
        {
            throw new InvalidInputException("That room has appointments scheduled, can't renovate");
        }

        System.Console.WriteLine("Input the first room that will split off:");
        var firstRoom = InputRoom();

        System.Console.WriteLine("Input the second room that will split off:");
        var secondRoom = InputRoom();

        if (firstRoom.Location == secondRoom.Location)
        {
            throw new InvalidInputException("Nope, can't have same location for both.");
        }

        var renovation = new SplitRenovation(originalRoom.Location, range, firstRoom, secondRoom);

        _hospital.SplitRenovationService.Schedule(renovation, firstRoom, secondRoom);
        System.Console.Write("Successfully scheduled split renovation. Input anything to continue >>  ");
    }
    
    private void DoMergeRenovation()
    {
        System.Console.WriteLine("Warning! Doing this will make any equipment inside inaccessible during renovation");
        System.Console.Write("This will move all equipment present at the beginning of the renovation ");
        System.Console.WriteLine("in first and second room into the merging room");
        System.Console.WriteLine("Move it first if you so desire");

        System.Console.Write("Input first number to merge >> ");
        var firstNumber = ReadInt(0, _loadedRooms.Count - 1);
        var firstRoom = _loadedRooms[firstNumber];

        System.Console.Write("Input second number to merge >> ");
        var secondNumber = ReadInt(0, _loadedRooms.Count - 1);
        var secondRoom = _loadedRooms[secondNumber];

        if (secondNumber == firstNumber)
        {
            throw new InvalidCastException("Nope, can't merge a room with itself.");
        }

        var range = InputDateRange();

        if (!_hospital.AppointmentRepo.IsRoomAvailableForRenovation(firstRoom.Location, range.Starts))
        {  
            // TODO: change exception type
            throw new InvalidInputException("First room has appointments scheduled, can't renovate.");
        }

        if (!_hospital.AppointmentRepo.IsRoomAvailableForRenovation(secondRoom.Location, range.Starts))
        {  
            throw new InvalidInputException("Second room has appointments scheduled, can't renovate.");
        }

        System.Console.WriteLine("Input the room that these will merge into:");
        var mergingRoom = InputRoom();

        var renovation = new MergeRenovation(range, firstRoom.Location, secondRoom.Location, mergingRoom.Location);

        _hospital.MergeRenovationService.Schedule(renovation, mergingRoom);
        System.Console.Write("Successfully scheduled merge renovation. Input anything to continue >>  ");
    }
}