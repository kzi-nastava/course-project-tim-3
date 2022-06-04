using HospitalSystem.Core.Utils;

namespace HospitalSystem.ConsoleUI;

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

[System.Serializable]
public class AbortException : System.Exception
{
    public AbortException() { }
    public AbortException(string message) : base(message) { }
    public AbortException(string message, System.Exception inner) : base(message, inner) { }
    protected AbortException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public abstract class ConsoleUI
{
    protected ConsoleUI()
    {

    }

    public abstract void Start();

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

    protected DateRange InputDateRange()
    {
        System.Console.Write("Input date-time when it starts >> ");
        var rawDate = ReadSanitizedLine();
        var starts = DateTime.Parse(rawDate);

        System.Console.Write("Input date-time when it ends >> ");
        rawDate = ReadSanitizedLine();
        var ends = DateTime.Parse(rawDate);

        return new DateRange(starts, ends, allowPast: false);
    }

    protected string ReadUpdate(string defaultVal)
    {
        var val = ReadSanitizedLine();
        if (val != "")
            return val;
        return defaultVal;
    }

    protected string ReadNotEmpty(string errMsg)
    {
        var val = ReadSanitizedLine();
        if (val == "")
            throw new InvalidInputException(errMsg);
        return val;
    }
}