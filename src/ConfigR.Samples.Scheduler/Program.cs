// <copyright file="Program.cs" company="ConfigR contributors">
//  Copyright (c) ConfigR contributors. (configr.net@gmail.com)
// </copyright>

using System.ServiceProcess;

namespace ConfigR.Samples.Scheduler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Common.Logging;
    using ConfigR;

    class Program : ServiceBase
    {
        public static void Main()
        {
            using (var service = new Program())
            {
                // so we can run interactive from Visual Studio or as a service
                if (Environment.UserInteractive)
                {
                    service.OnStart(null);
                    Console.WriteLine("\r\nPress enter key to stop program\r\n");
                    Console.ReadLine();
                    service.OnStop();
                    return;
                }
                Run(service);
            }
        }


        Dictionary<Schedule, Timer> timers;

        protected override void OnStart(string[] args)
        {   
            // NOTE (Adam): I purposely omitted some stuff which ought to be in here to avoid cluttering the sample, including:
            // * thread safety between schedule execution and timer disposal
            // * initialization/schedule execution overlapping with next dueTime - currently an exception would be thrown due to negative dueTime
            var schedules = Config.Global.Get<Schedule[]>("Schedules");

            timers = schedules.ToDictionary(
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
                            LogManager.GetCurrentClassLogger().Error("Error executing schedule", ex);
                        }

                        timers[schedule].Change(((schedule.NextRun += schedule.RepeatInterval) - DateTime.Now).OrNow(), TimeSpan.FromMilliseconds(-1));
                    },
                    null,
                    (schedule.NextRun - DateTime.Now).OrNow(),
                    TimeSpan.FromMilliseconds(-1)));
        }

        protected override void OnStop()
        {
            timers.Values.ToList().ForEach(timer => timer.Dispose());
        }

    }
}