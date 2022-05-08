namespace Hospital;

[System.Serializable]
public class QuitToMainMenuException : System.Exception
{
    public QuitToMainMenuException() { }
    public QuitToMainMenuException(string message) : base(message) { }
    public QuitToMainMenuException(string message, System.Exception inner) : base(message, inner) { }
    protected QuitToMainMenuException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

[System.Serializable]
public class InvalidInputException : System.Exception
{
    public InvalidInputException() { }
    public InvalidInputException(string message) : base(message) { }
    public InvalidInputException(string message, System.Exception inner) : base(message, inner) { }
    protected InvalidInputException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public abstract class ConsoleUI
{
    protected Hospital _hospital;
    protected User? _user;

    public abstract void Start();

    public ConsoleUI(Hospital hospital)
    {
        this._hospital = hospital;
    }

    public string ReadSanitizedLine()
    {
        var raw = System.Console.ReadLine();
        string sanitized;
        if (raw is null)
            sanitized = "";
        else
            sanitized = raw.ToLower();
        return sanitized;
    }

    // the bounds are both inclusive
    public int ReadInt(int lowerBound = Int32.MinValue, int upperBound = Int32.MaxValue,
                       string errorMessageBounds = "NUMBER OUT OF BOUNDS!",
                       string errorMessageWrongInput = "NUMBER NOT RECOGNIZED!")
    {
        var rawNumber = ReadSanitizedLine();
        bool success = Int32.TryParse(rawNumber, out int number);
        
        if (!success)
            throw new InvalidInputException(errorMessageWrongInput);

        if (number < lowerBound || number > upperBound)
            throw new InvalidInputException(errorMessageBounds);

        return number;
    }
}