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
}