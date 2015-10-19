#r "ConfigR.Samples.Scheduler.exe"
#r "NLog.dll"

using System;
using ConfigR.Samples.Scheduler;
using NLog;

Add(
    "Schedules",
    new[]
    {
        new Schedule
        {
            Action = () =>
                {
                    LogManager.GetCurrentClassLogger().Info("The 1st schedule is sending some emails!");
                    // send some emails
                },
            NextRun = DateTime.Now.AddSeconds(2),
            RepeatInterval = TimeSpan.FromSeconds(4),
        },
        new Schedule
        {
            Action = () =>
                {
                    LogManager.GetCurrentClassLogger().Info("The 2nd schedule is downloading some reports!");
                    // download some reports
                },
            NextRun = DateTime.Now.AddSeconds(3),
            RepeatInterval = TimeSpan.FromSeconds(4),
        },
        new Schedule
        {
            Action = () =>
                {
                    LogManager.GetCurrentClassLogger().Info("The 3rd schedule is performing some housekeeping!");
                    // perform some housekeeping
                },
            NextRun = DateTime.Now.AddSeconds(4),
            RepeatInterval = TimeSpan.FromSeconds(4),
        }
    });