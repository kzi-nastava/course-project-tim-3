namespace Hospital
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hospital = new Hospital();

            // REVIEW: Da li treba onlyGenerate uopste?
            if (args.Count() == 1 && args[0] == "-onlyGenerate")
            {
                TestGenerator.Generate(hospital);
                return;
            }
            else if (args.Count() == 1 && args[0] == "-generate")
            {
                TestGenerator.Generate(hospital);
            }
            var ui = new HospitalUI(hospital);
            ui.Start();
        }
    }
}