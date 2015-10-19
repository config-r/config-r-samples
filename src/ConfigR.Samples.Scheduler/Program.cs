// <copyright file="Program.cs" company="ConfigR contributors">
//  Copyright (c) ConfigR contributors. (configr.net@gmail.com)
// </copyright>

namespace ConfigR.Samples.Scheduler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using ConfigR;
    using NLog;
    using Topshelf;

    public static class Program
    {
        public static void Main(string[] args)
        {
            // NOTE (Adam): I purposely ommitted some stuff which ought to be in here to avoid cluttering the sample, including:
            // * thread safety between schedule execution and timer disposal
            // * initialization/schedule execution overlapping with next dueTime - currently an exception would be thrown due to negative dueTime
            HostFactory.Run(h => h.Service<Schedule[]>(s =>
            {
                s.ConstructUsing(() => Config.Global.Get<Schedule[]>("Schedules"));

                Dictionary<Schedule, Timer> timers = null;
                s.WhenStarted(schedules => timers = schedules.ToDictionary(
                    schedule => schedule,
                    schedule => new Timer(
                        state =>
                        {
                            try
                            {
                                schedule.Action();
                            }
                            catch (Exception ex)
                            {
                                LogManager.GetCurrentClassLogger().Error(ex, "Error executing schedule");
                            }

                            timers[schedule].Change(((schedule.NextRun += schedule.RepeatInterval) - DateTime.Now).OrNow(), TimeSpan.FromMilliseconds(-1));
                        },
                        null,
                        (schedule.NextRun - DateTime.Now).OrNow(),
                        TimeSpan.FromMilliseconds(-1))));

                s.WhenStopped(schedules => timers.Values.ToList().ForEach(timer => timer.Dispose()));
            }));
        }

        private static TimeSpan OrNow(this TimeSpan timeSpan)
        {
            return timeSpan.TotalMilliseconds < 0 ? new TimeSpan(0) : timeSpan;
        }
    }
}
