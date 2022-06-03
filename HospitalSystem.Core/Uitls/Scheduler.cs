using Timer = System.Timers.Timer;

namespace HospitalSystem.Core.Utils;

public static class Scheduler
{
    public static void Schedule(DateTime invokeAt, Action action)
    {
        var waiting = invokeAt - DateTime.Now;
        if (waiting < TimeSpan.Zero)  // if task is past, just do it
        {
            action();
            return;
        }
        var timer = new Timer(waiting.TotalMilliseconds);
        timer.Elapsed += ((sender, args) => action());
        timer.AutoReset = false;
        timer.Enabled = true;
    }
}