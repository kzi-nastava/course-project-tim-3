using System.Text.RegularExpressions;

namespace HospitalSystem;

[System.Serializable]
public class InvalidTokenException : System.Exception
{
    public InvalidTokenException() { }
    public InvalidTokenException(string message) : base(message) { }
    public InvalidTokenException(string message, System.Exception inner) : base(message, inner) { }
    protected InvalidTokenException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public struct EquipmentQuery
{
    public int? MinCount { get; set; }
    public int? MaxCount { get; set; }
    public EquipmentType? Type { get; set; }
    public Regex? NameContains { get; set; }

    public EquipmentQuery(string query)
    {
        // TODO: make it so repeated same will throw error
        MinCount = null;
        MaxCount = null;
        Type = null;
        NameContains = null;
        if (query == "")
            return;
        var tokens = query.Split();
        foreach (var token in tokens)
        {
            if (token.StartsWith("min:"))
            {
                bool success = Int32.TryParse(token.Substring(4), out int number);
                if (!success)
                    throw new InvalidTokenException("Given min is not a number.");
                MinCount = number;
            } 
            else if (token.StartsWith("max:"))
            {
                bool success = Int32.TryParse(token.Substring(4), out int number);
                if (!success)
                    throw new InvalidTokenException("Given max is not a number.");
                MaxCount = number;
            }
            else if (token.StartsWith("type:"))
            {
                EquipmentType type;
                var success = Enum.TryParse(token.Substring(5), true, out type);
                if (!success)
                    throw new InvalidTokenException("Not a valid type.");
                Type = type;
            }
            else
            {
                throw new InvalidTokenException("Unrecognized token: " + token);
            }
        }
    }
}