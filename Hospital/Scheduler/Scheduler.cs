using Timer = System.Timers.Timer;

namespace Hospital;

public delegate void Task();

public static class Scheduler
{
    public static void Schedule(DateTime invokeAt, Task task)
    {
        var waiting = invokeAt - DateTime.Now;
        if (waiting < TimeSpan.Zero)  // if task is past, just do it
        {
            task();
            return;
        }
        var timer = new Timer(waiting.TotalMilliseconds);
        timer.Elapsed += ((sender, args) => task());
        timer.AutoReset = false;
        timer.Enabled = true;
    }
}