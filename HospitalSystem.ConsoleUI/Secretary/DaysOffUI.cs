using HospitalSystem.Core;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Globalization;

namespace HospitalSystem.ConsoleUI;

public class DaysOffUI : HospitalClientUI
{

    public DaysOffUI(Hospital hospital) : base(hospital){}

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("INPUT OPTIONS:");
             System.Console.WriteLine("   1. View requests-(vr)" );
             System.Console.WriteLine("   2. Quit-(q)");
             System.Console.WriteLine("   3. Exit-(x)");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            try
            {
                if (choice == "view requests" || choice == "v")
                {
                    ViewRequests();
                }
                else if (choice == "quit" || choice == "q")
                {
                    throw new QuitToMainMenuException("From StartManageEquipments");
                }
                else if (choice == "exit" || choice == "x")
                {
                    System.Environment.Exit(0);
                }
                else
                {
                    System.Console.WriteLine("Invalid input - read the available commands!");
                    System.Console.Write("Intput anything to continue >> ");
                    ReadSanitizedLine();
                }
            }
            catch (InvalidInputException e)
            {
                System.Console.Write(e.Message + " Intput anything to continue >> ");
                ReadSanitizedLine();
            }
            catch (FormatException e)
            {
                System.Console.Write(e.Message + " Intput anything to continue >> ");
                ReadSanitizedLine();
            }
        }
    }

    public void ViewRequests(){
        
    }
}

