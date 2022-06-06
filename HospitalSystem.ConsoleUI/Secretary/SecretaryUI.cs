// using MongoDB.Driver;
// using MongoDB.Bson;
using HospitalSystem.Core;

namespace HospitalSystem.ConsoleUI;
// [System.Serializable]

public class SecretaryUI : ConsoleUI
{   
    // public SecretaryUI(Hospital _hospital, User? _user) : base(_hospital) 
    public SecretaryUI(Hospital _hospital) : base(_hospital) 
    {
        // this._user = _user;
    }

        public override void Start()
    {
        while (true)
        {
            try
            {
                System.Console.Clear();
                System.Console.WriteLine("Available commands:");
                System.Console.WriteLine("  1. CRUD options");
                System.Console.WriteLine("  2. Requests");
                System.Console.WriteLine("  3. Schedule");
                System.Console.WriteLine("  4. SOS schedule");
                System.Console.WriteLine("  5. Equipment options");
                System.Console.WriteLine("  6. Log out");
                System.Console.WriteLine("  7. Exit");
                System.Console.Write(">> ");
                var choice = ReadSanitizedLine();
                if (choice == "crud options" || choice == "cr")
                {
                    var crudUI = new CrudUI(_hospital);
                    crudUI.Start();
                }
                else if(choice == "requests" || choice == "re")
                {
                    continue;
                }
                else if(choice == "schedule" || choice == "sc")
                {
                    continue;
                }
                else if(choice == "sos schedule" || choice == "sos")
                {
                    continue;
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
                    System.Console.Write("UNRECOGNIZED OPTION. INPUT ANYTHING TO CONTINUE >> ");
                    ReadSanitizedLine();
                }
            }
            catch (QuitToMainMenuException)
            {

            }
        }
    }
}

