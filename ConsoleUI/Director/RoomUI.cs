using HospitalSystem.Utils;

namespace HospitalSystem;

public class RoomUI : ConsoleUI
{
    private List<Room> _loadedRooms;

    public RoomUI(Hospital hospital) : base(hospital)
    {
        _loadedRooms = _hospital.RoomRepo.GetAll().ToList();
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
            catch (FormatException e)
            {
                System.Console.Write(e.Message + " INPUT ANYTHING TO CONTINUE >> ");
            }
            _loadedRooms = _hospital.RoomRepo.GetAll().ToList();
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
        System.Console.Write("INPUT NUMBER >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);
        var room = _loadedRooms[number];

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
        var newRoom = InputRoom();
        _hospital.RoomRepo.Add(newRoom);
        System.Console.Write("SUCCESSFULLY ADDED ROOM. INPUT ANYTHING TO CONTINUE >> ");
    }

    private Room InputRoom()
    {
        System.Console.Write("ENTER ROOM LOCATION >> ");
        var location = ReadSanitizedLine();
        if (location == "")
            throw new InvalidInputException("INVALID LOCATION!");
        if (_hospital.RoomRepo.DoesExist(location))
            throw new InvalidInputException("ROOM WITH THAT LOCATION ALREADY EXISTS!");

        System.Console.Write("ENTER ROOM NAME >> ");
        var name = ReadSanitizedLine();
        if (name == "")
            throw new InvalidInputException("INVALID NAME!");

        System.Console.Write("ENTER ROOM TYPE [rest|operation|checkup|other] >> ");
        var rawType = ReadSanitizedLine();
        bool success = Enum.TryParse(rawType, true, out RoomType type);
        if (!success || type == RoomType.STOCK)
            throw new InvalidInputException("NOT A VALID TYPE!");

        return new Room(location, name, type);
    }

    private void Delete()
    {
        System.Console.Write("INPUT NUMBER >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);
        if (_hospital.EquipmentRepo.GetAllIn(_loadedRooms[number].Location).Any())
        {
            // TODO: make into a moving equipment submenu
            System.Console.Write("THIS ROOM HAS EQUIPMENT IN IT. THIS OPERATION WILL DELETE IT ALL. ARE YOU SURE? [y/N] >> ");
            var answer = ReadSanitizedLine();
            if (answer != "y")
                throw new AbortException("NOT A YES. ABORTING.");
            _hospital.EquipmentRepo.DeleteAllInRoom(_loadedRooms[number]);
        }
        _hospital.RoomRepo.Delete(_loadedRooms[number].Location);
        System.Console.Write("SUCCESSFULLY DELETED ROOM. INPUT ANYTHING TO CONTINUE >> ");
    }

    private void DoSimpleRenovation()
    {
        System.Console.WriteLine("WARNING! Doing this will make any equipment inside inaccessible during renovation. ");
        System.Console.WriteLine("Move it first if you so desire");
        System.Console.Write("INPUT NUMBER >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);

        var range = InputDateRange();

        if (!_hospital.AppointmentRepo.IsRoomAvailableForRenovation(_loadedRooms[number].Location, range.Starts))
        {
            throw new InvalidInputException("THAT ROOM HAS APPOINTMENTS SCHEDULED, CAN'T RENOVATE");
        }

        var renovation = new SimpleRenovation(_loadedRooms[number].Location, range);
        _hospital.SimpleRenovationRepo.Add(renovation);
        _hospital.SimpleRenovationRepo.Schedule(renovation);
        System.Console.Write("SUCCESSFULLY SCHEDULED SIMPLE RENOVATION. INPUT ANYTHING TO CONTINUE >>  ");
    }

    private void DoSplitRenovation()
    {
        System.Console.WriteLine("WARNING! Doing this will make any equipment inside inaccessible during renovation");
        System.Console.WriteLine("This will move all equipment present at the beginning of the renovation into the first room");
        System.Console.WriteLine("Move it first if you so desire");
        System.Console.Write("INPUT NUMBER TO SPLIT >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);
        var originalRoom = _loadedRooms[number];

        var range = InputDateRange();

        if (!_hospital.AppointmentRepo.IsRoomAvailableForRenovation(originalRoom.Location, range.Starts))
        {
            throw new InvalidInputException("THAT ROOM HAS APPOINTMENTS SCHEDULED, CAN'T RENOVATE");
        }

        System.Console.WriteLine("INPUT THE FIRST ROOM THAT WILL SPLIT OFF:");
        var firstRoom = InputRoom();

        System.Console.WriteLine("INPUT THE SECOND ROOM THAT WILL SPLIT OFF:");
        var secondRoom = InputRoom();

        if (firstRoom.Location == secondRoom.Location)
        {
            throw new InvalidInputException("NOPE, CAN'T HAVE SAME LOCATION FOR BOTH!");
        }

        var renovation = new SplitRenovation(originalRoom.Location, range, firstRoom, secondRoom);

        // TODO: put this below in a service
        _hospital.RoomRepo.AddInactive(firstRoom);
        _hospital.RoomRepo.AddInactive(secondRoom);
        _hospital.SplitRenovationRepo.Add(renovation);
        _hospital.SplitRenovationRepo.Schedule(renovation);
        System.Console.Write("SUCCESSFULLY SCHEDULED SPLIT RENOVATION. INPUT ANYTHING TO CONTINUE >>  ");
    }
    
    private void DoMergeRenovation()
    {
        System.Console.WriteLine("WARNING! Doing this will make any equipment inside inaccessible during renovation");
        System.Console.Write("This will move all equipment present at the beginning of the renovation ");
        System.Console.WriteLine("in first and second room into the merging room");
        System.Console.WriteLine("Move it first if you so desire");

        System.Console.Write("INPUT FIRST NUMBER TO MERGE >> ");
        var firstNumber = ReadInt(0, _loadedRooms.Count - 1);
        var firstRoom = _loadedRooms[firstNumber];

        System.Console.Write("INPUT FIRST NUMBER TO MERGE >> ");
        var secondNumber = ReadInt(0, _loadedRooms.Count - 1);
        var secondRoom = _loadedRooms[secondNumber];

        if (secondNumber == firstNumber)
        {
            throw new InvalidCastException("NOPE, CAN'T MERGE A ROOM WITH ITSELF");
        }

        var range = InputDateRange();

        if (!_hospital.AppointmentRepo.IsRoomAvailableForRenovation(firstRoom.Location, range.Starts))
        {  
            // TODO: change exception type
            throw new InvalidInputException("FIRST ROOM HAS APPOINTMENTS SCHEDULED, CAN'T RENOVATE.");
        }

        if (!_hospital.AppointmentRepo.IsRoomAvailableForRenovation(secondRoom.Location, range.Starts))
        {  
            throw new InvalidInputException("SECOND ROOM HAS APPOINTMENTS SCHEDULED, CAN'T RENOVATE.");
        }

        System.Console.WriteLine("INPUT THE ROOM THAT THESE WILL MERGE INTO:");
        var mergingRoom = InputRoom();

        var renovation = new MergeRenovation(range, firstRoom.Location, secondRoom.Location, mergingRoom.Location);

        // TODO: put this below in a service
        _hospital.RoomRepo.AddInactive(mergingRoom);
        _hospital.MergeRenovationRepo.Add(renovation);
        _hospital.MergeRenovationRepo.Schedule(renovation);
        System.Console.Write("SUCCESSFULLY SCHEDULED MERGE RENOVATION. INPUT ANYTHING TO CONTINUE >>  ");
    }
}