using MongoDB.Bson.Serialization.Attributes;

namespace HospitalSystem.Core.Utils;

public class DateRange
{
    // the minimum allowed range (makes scheduling easy)
    [BsonIgnore]
    private static TimeSpan s_minDuration = TimeSpan.FromSeconds(60);  
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    [BsonElement]
    public DateTime Starts { get; }
    // TODO: these local kinds everywhere will cause a bug if whole system changes timezone. Unlikely, but still...
    [BsonElement]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime Ends { get; }

    public DateRange(DateTime starts, DateTime ends, bool allowPast) : this(starts, ends)
    {
        if (!allowPast && starts < DateTime.Now)
        {
            throw new ArgumentException("Can not create range that starts in the past.");
        }
    }

    [BsonConstructor]
    internal DateRange(DateTime starts, DateTime ends)
    {
        if (ends < starts)
        {
            throw new ArgumentException("Range can not end before it starts.");
        }

        if (ends - starts < s_minDuration)
        {
            throw new ArgumentException("Can not make range that lasts less than the minimum duration: " 
                + s_minDuration.Seconds + " seconds.");
        }

        Starts = starts;
        Ends = ends;
    }

    public override string ToString()
    {
        return Starts + " - " + Ends;
    }

    public bool Contains(DateTime date)
    {
        return Starts <= date && date <= Ends;
    }

    public bool Overlaps(DateRange range)
    {
        return Starts <= range.Ends && range.Starts <= Ends;
    }

    public TimeSpan GetDuration()
    {
        return Ends - Starts;
    }

    public bool HasPassed()
    {
        return Ends < DateTime.Now;
    }

    public bool IsFuture()
    {
        return Starts > DateTime.Now;
    }
    
}