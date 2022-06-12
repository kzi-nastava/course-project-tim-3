namespace HospitalSystem.ConsoleUI.Director;

public class IngredientsUI : ConsoleUI
{
    private List<string> _ingredients;

    public IngredientsUI(List<string> ingredients)
    {
        _ingredients = ingredients;
    }

    public override void Start()
    {
        while (true)
        {
            System.Console.Clear();
            System.Console.WriteLine("--- Editing ingredients ---");
            DisplayIngredients();
            System.Console.WriteLine(@"
            INPUT OPTION:
                [add|a] Add ingredients
                [remove|r] Remove ingredient
                [edit|e] Edit an ingredient
                [done|d] Finish editing ingredients
                [quit|q] Quit to main menu
                [exit|x] Exit program
            ");
            System.Console.Write(">> ");
            var choice = ReadSanitizedLine();
            if (choice == "a" || choice == "add")
            {
                AddIngredients();
            }
            else if (choice == "r" || choice == "remove")
            {
                RemoveIngredient();
            }
            else if (choice == "e" || choice == "edit")
            {
                EditIngredient();
            }
            else if (choice == "d" || choice == "done")
            {
                if (_ingredients.Count == 0)
                {
                    // TODO: move this to some med service
                    throw new InvalidInputException("Can not have no ingredients.");
                }
                return;
            }
            else if (choice == "q" || choice == "quit")
            {
                throw new QuitToMainMenuException("From StartManageMedicationRequests.");
            }
            else if (choice == "x" || choice == "exit")
            {
                System.Environment.Exit(0);
            }
            else
            {
                System.Console.WriteLine("Invalid input - please read the available commands.");
                System.Console.Write("Input anything to continue >> ");
                ReadSanitizedLine();
            }
        }
    }

    private void DisplayIngredients()
    {
        System.Console.WriteLine("No. | Ingredient");
        for (int i = 0; i < _ingredients.Count; i++)
        {
            System.Console.WriteLine(i + " | " + _ingredients[i]);
        }
    }

    private void RemoveIngredient()
    {
        System.Console.Write("Input number to remove >> ");
        var num = ReadInt(0, _ingredients.Count - 1);
        _ingredients.RemoveAt(num);
    }

    private void EditIngredient()
    {
        System.Console.Write("Input number to edit >> ");
        var num = ReadInt(0, _ingredients.Count - 1);
        System.Console.Write("Input new ingredient >> ");
        var ingredient = ReadNotEmpty("Ingredient can not be empty.");
        _ingredients[num] = ingredient;
    }

    private void AddIngredients()
    {
        System.Console.WriteLine("Input new ingredients. Each line is one ingredient. " + 
            "Empty line means that you are done with inputting new ingredients");
        var ingredient = "INVALID";
        while (ingredient != "")
        {
            System.Console.Write(">> ");
            ingredient = ReadSanitizedLine();
            if (ingredient != "")
            {
                _ingredients.Add(ingredient);
            }
        }
    }
}