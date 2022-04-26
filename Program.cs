using MongoDB.Bson;

namespace Hospital
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ui = new HospitalUI();
            // generate tests TODO: move this to dedicated teting interface
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
                File.AppendAllText("db/users.json", user.ToBsonDocument().ToJson());
            }
            ui.Start();
        }
    }
}