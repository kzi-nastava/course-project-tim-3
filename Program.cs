using MongoDB.Bson;
namespace Hospital
{
    class Program
    {
        static void Main(string[] args)
        {
            var ui = new HospitalUI();
            // generate tests
            for (int i = 0; i < 100; i++)
            {
                User user;
                if (i % 4 == 0)
                    user = new User("a" + i, "a" + i, Role.DIRECTOR);
                else if (i % 4 == 1)
                    user = new User("a" + i, "a" + i, Role.DOCTOR);
                else if (i % 4 == 2)
                    user = new User("a" + i, "a" + i, Role.PATIENT);
                else
                    user = new User("a" + i, "a" + i, Role.SECRETARY);
                ui.AddUser(user.username, user.password, user.role);
                File.AppendAllText("db/users.json", user.ToBsonDocument().ToJson());
            }
            ui.Start();
        }
    }
}