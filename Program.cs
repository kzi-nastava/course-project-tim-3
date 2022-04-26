using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace Hospital
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ui = new ConsoleUI();
            // generate tests TODO: move this to dedicated teting interface
            var hospitalContents = new {Users = new List<User>()};
            for (int i = 0; i < 100; i++)
            {
                User user;
                if (i % 4 == 0)
                    user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.DIRECTOR);
                else if (i % 4 == 1)
                    user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.DOCTOR);
                else if (i % 4 == 2)
                    user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.PATIENT);
                else
                    user = new User("a" + i, "a" + i, "imenko" + i, "prezimenic" + i, Role.SECRETARY);
                ui.AddUser(user.Username, user.Password, user.Person.FirstName, user.Person.LastName, user.Role);
                hospitalContents.Users.Add(user);
                File.WriteAllText("db/hospital.json", hospitalContents.ToBsonDocument().ToJson(
                    new JsonWriterSettings {Indent = true}));
            }
            ui.Start();
        }
    }
}