using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;

public class SecretaryUI : HospitalClientUI
{   
    public SecretaryUI(Hospital _hospital) : base(_hospital){}

    public override void Start()
    {
        while (true)
        {
            try
            {
                System.Console.Clear();
                System.Console.WriteLine("Available commands:");
                System.Console.WriteLine("   1. Patients options-(po)");
                System.Console.WriteLine("   2. Checkup options-(co)");
                System.Console.WriteLine("   3. Equipment options-(eq)");
                System.Console.WriteLine("   4. Log out-(lo)");
                System.Console.WriteLine("   5. Exit-(x)");
                System.Console.Write(">> ");
                var choice = ReadSanitizedLine();
                if (choice == "patients options" || choice == "po")
                {
                    var crudUI = new CrudUI(_hospital);
                    crudUI.Start();
                }
                else if(choice == "Checkup options" || choice == "co")
                {
                    var checkupUI = new CheckupUI(_hospital);
                    checkupUI.Start();
                }
                else if(choice == "equipment options" || choice == "eq")
                {
                    var equipUI = new EquipUI(_hospital);
                    equipUI.Start();
                }
                else if (choice == "log out" || choice == "lo")
                {
                    return;
                }
                else if (choice == "exit" || choice == "x")
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    System.Console.Write("Unrecognized option. Intput anything to continue >> ");
                    ReadSanitizedLine();
                }
            }
            catch (QuitToMainMenuException){}
        }
    }
}

