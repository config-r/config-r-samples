using System;

static class TimeSpanExtensions
{
    public static TimeSpan OrNow(this TimeSpan timeSpan)
    {
        return timeSpan.TotalMilliseconds < 0 ? new TimeSpan(0) : timeSpan;
    }
}