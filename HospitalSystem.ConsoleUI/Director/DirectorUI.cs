using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI.Director;

public class DirectorUI : HospitalClientUI
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
                    [manage medication requests|mmr] Manage medication requests
                    [log out|lo] Log out
                    [exit|x] Exit program
                ");
                System.Console.Write(">> ");
                var choice = ReadSanitizedLine();
                if (choice == "mr" || choice == "manage rooms")
                {
                    var roomUI = new RoomUI(_hospital);
                    roomUI.Start();
                }
                else if (choice == "me" || choice == "manage equipment")
                {
                    var equipmentUI = new EquipmentUI(_hospital);
                    equipmentUI.Start();
                }
                else if (choice == "mmr" || choice == "manage medication requests")
                {
                    var medicationRequestUI = new MedicationRequestUI(_hospital);
                    medicationRequestUI.Start();
                }
                else if (choice == "lo" || choice == "log out")
                {
                    return;
                }
                else if (choice == "x" || choice == "exit")
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    System.Console.Write("Unrecognized option. Input anything to continue >> ");
                    ReadSanitizedLine();
                }
            }
            catch (QuitToMainMenuException)
            {

            }
        }
    }
}