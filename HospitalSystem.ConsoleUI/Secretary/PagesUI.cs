using HospitalSystem.Core;
using MongoDB.Bson;

namespace HospitalSystem.ConsoleUI;

public class PagesUI : HospitalClientUI
{
    List<User> patients;
    int size;
    int startIndex = 0;
    int endIndex = 10;

    public PagesUI(Hospital hospital) : base(hospital)
    {
        patients = _hospital.UserService.GetPatients().ToList();
        size = patients.Count();
    }

    public override void Start()
    {   
        Console.Clear();

        Header();
        Page(patients, startIndex, endIndex);   

        while(true){
            string selectedOption = SelectOption();
            Console.Clear();
            if (selectedOption == "left")
            {
                MoveLeft();
            }
            else if(selectedOption == "right")
            {   
                MoveRight();
            }
            else{
                return;
            }
        }
    }

    public void MoveLeft()
    {
        startIndex = startIndex-10;
        endIndex = endIndex-10;
        
        if(startIndex >= 0)
        { 
            Header();
            Page(patients, startIndex, endIndex);
        }
        else
        {
            startIndex = startIndex+10;
            endIndex = endIndex+10;
            Header();
            Page(patients, startIndex, endIndex);
            System.Console.WriteLine("There are no more previous pages.");
        }
    }

    public void MoveRight()
    {
        startIndex = startIndex+10;
        endIndex = endIndex+10;

        if(endIndex <= size)
        {   
            Header();
            Page(patients, startIndex, endIndex);
        }
        else if((10 - (endIndex-size)) >= 0)
        {
            int newEndIndex = 10 - (endIndex-size);
            Header();
            Page(patients, startIndex, size);
        }
        else
        {
            Header();
            Page(patients, startIndex-10, size);
            startIndex = startIndex-10;
            endIndex = endIndex-10;
            System.Console.WriteLine("There are no more next pages.");
        }
    }

    public string SelectOption()
    {
        Console.Write("Please enter a command: ");
        string input = ReadSanitizedLine();
        return input.Trim();
    }

    public void Header()
    {
        System.Console.WriteLine("__________________________________________________________________________________________");
        System.Console.WriteLine("|                       |                      |                                          |");
        System.Console.WriteLine("|       First Name      |      Last Name       |                   Email                  |");
        System.Console.WriteLine("|_______________________|______________________|__________________________________________|");
        System.Console.WriteLine("|                       |                      |                                          |");

    }

    public void Page(List<User> patientAccounts, int startIndex, int endIndex)
    {   
        for(var i = startIndex; i < endIndex; i++ ){
            var patientAccount = patientAccounts[i];
            Patient pat = _hospital.PatientService.GetPatientById((ObjectId) patientAccount.Person.Id);
            System.Console.WriteLine(String.Format("| {0,-21} | {1,-20} | {2, -40} |", pat.FirstName, pat.LastName, patientAccount.Email));
        }

        System.Console.WriteLine("|_______________________|______________________|__________________________________________|");
        System.Console.WriteLine("Choose:");
        System.Console.WriteLine("       <Left> (previous 10 users)");
        System.Console.WriteLine("       <Right> (next 10 users)");
        System.Console.WriteLine("       <Back>");
        System.Console.WriteLine("");
    }
}