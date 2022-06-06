using System.Text.RegularExpressions;
using HospitalSystem.Core;
using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalSystem.ConsoleUI;
[System.Serializable]

public class PagesUI : ConsoleUI
{
    public PagesUI(Hospital hospital) : base(hospital){}

    public override void Start()
    {   
        Console.Clear();
        UserService us= _hospital.UserService;
        IQueryable<User> list = us.GetPatients();
        List<User> users = new List<User>();
        foreach(var u in list){
            users.Add(u);
        }
        
        int usersListSize = users.Count();
        int startIndex = 0;
        int endIndex = 10;

        header();
        page(users, startIndex, endIndex);   

        while(true){
            string selectedOption = selectOption();
            Console.Clear();
            if (selectedOption == "left")
            {
                startIndex = startIndex-10;
                endIndex = endIndex-10;
                if(startIndex >= 0)
                { 
                    header();
                    page(users, startIndex, endIndex);
                }
                else{
                    startIndex = startIndex+10;
                    endIndex = endIndex+10;
                    header();
                    page(users, startIndex, endIndex);
                    System.Console.WriteLine("There are no more previous pages");
                }
            }
            else if(selectedOption == "right")
            {   
                startIndex = startIndex+10;
                endIndex = endIndex+10;
                if(endIndex <= usersListSize)
                {   
                    header();
                    page(users, startIndex, endIndex);
                }
                else if((10 - (endIndex-usersListSize)) >= 0){
                    int newEndIndex = 10 - (endIndex-usersListSize);
                    header();
                    page(users, startIndex, usersListSize);
                }
                else{
                    header();
                    page(users, startIndex-10, usersListSize);
                    startIndex = startIndex-10;
                    endIndex = endIndex-10;
                    System.Console.WriteLine("There are no more next pages");
                }
            }
            else if(selectedOption == "back"){
                return;
            }
        }
    }

    public string selectOption()
    {
        Console.Write("Please enter a command: ");
        string? input = Console.ReadLine();
        if (input is null)
        {
            throw new NullInputException("Null value as input");
        }
        return input.ToLower().Trim();
    }

        public void header()
    {
        System.Console.WriteLine("__________________________________________________________________________________________");
        System.Console.WriteLine("|                       |                      |                                          |");
        System.Console.WriteLine("|       First Name      |      Last Name       |                   Email                  |");
        System.Console.WriteLine("|_______________________|______________________|__________________________________________|");
        System.Console.WriteLine("|                       |                      |                                          |");

    }

        public void page(List<User> usersList, int startIndex, int endIndex)
    {   
        int i;
        for(i = startIndex; i < endIndex; i++ ){
            var user = usersList.ElementAt(i);
            Patient pat = _hospital.PatientRepo.GetPatientById((ObjectId) user.Person.Id);
            System.Console.WriteLine(String.Format("| {0,-21} | {1,-20} | {2, -40} |", pat.FirstName, pat.LastName, user.Email));
        }

        System.Console.WriteLine("|_______________________|______________________|__________________________________________|");
        System.Console.WriteLine("Choose:");
        System.Console.WriteLine("       <Left> (previous 10 users)");
        System.Console.WriteLine("       <Right> (next 10 users)");
        System.Console.WriteLine("       <Back>");
        System.Console.WriteLine("");

    }
}