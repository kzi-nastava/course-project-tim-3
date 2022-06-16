
namespace HospitalSystem.Core.Utils;

public static class Globals
{
    public static DateTime OpeningTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);
    public static DateTime ClosingTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0);
    public static TimeSpan _checkupDuration = new TimeSpan(0,0,15,0);
    
}