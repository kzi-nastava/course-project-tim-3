using HospitalSystem.Core;
using HospitalSystem.Core.Renovations;

namespace HospitalSystem.ConsoleUI.Director;

public class RenovationUI : RoomUI
{
    public RenovationUI(Hospital hospital, List<Room> loadedRooms) : base(hospital, loadedRooms)
    {

    }

    public override void Start()
    {
        throw new NotImplementedException("Whoops");  // TODO: don't have such an inheritance, it bad
    }

    public void ScheduleSimple()
    {
        System.Console.WriteLine("Warning! Doing this will make any equipment inside inaccessible during renovation. ");
        System.Console.WriteLine("Move it first if you so desire");
        System.Console.Write("Input number >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);

        var range = InputDateRange();

        var renovation = new Renovation(range,
            new List<string>() {_loadedRooms[number].Location},
            new List<string>() {_loadedRooms[number].Location});
        _hospital.RenovationService.Schedule(renovation, new List<Room>());
        System.Console.Write("Successfully scheduled simple renovation.");
    }

    public void ScheduleSplit()
    {
        System.Console.WriteLine("Warning! Doing this will make any equipment inside inaccessible during renovation");
        System.Console.WriteLine("This will move all equipment present at the beginning of the renovation into the first room");
        System.Console.WriteLine("Move it first if you so desire");
        System.Console.Write("Input number to split >> ");
        var number = ReadInt(0, _loadedRooms.Count - 1);
        var originalRoom = _loadedRooms[number];

        var range = InputDateRange();

        System.Console.WriteLine("Input the first room that will split off:");
        var firstRoom = InputRoom();

        System.Console.WriteLine("Input the second room that will split off:");
        var secondRoom = InputRoom();

        var renovation = new Renovation(range,
            new List<string>() {originalRoom.Location},
            new List<string>() {firstRoom.Location, secondRoom.Location});
        _hospital.RenovationService.Schedule(renovation, new List<Room>() {firstRoom, secondRoom});
        System.Console.Write("Successfully scheduled split renovation.");
    }

    public void ScheduleMerge()
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

        var range = InputDateRange();

        System.Console.WriteLine("Input the room that these will merge into:");
        var mergingRoom = InputRoom();

        var renovation = new Renovation(range,
            new List<string>() {firstRoom.Location, secondRoom.Location},
            new List<string>() {mergingRoom.Location});
        _hospital.RenovationService.Schedule(renovation, new List<Room> {mergingRoom});
        System.Console.Write("Successfully scheduled merge renovation.");
    }
}