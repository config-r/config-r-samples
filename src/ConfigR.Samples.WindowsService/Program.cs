// <copyright file="Program.cs" company="ConfigR contributors">
//  Copyright (c) ConfigR contributors. (configr.net@gmail.com)
// </copyright>

namespace ConfigR.Samples.WindowsService
{
    using System;
    using ConfigR;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using Topshelf;

    public static class Program
    {
        public static void Main(string[] args)
        {
            using (var target = new ColoredConsoleTarget())
            {
                var loggingConfig = new LoggingConfiguration();
                loggingConfig.AddTarget("console", target);
                loggingConfig.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Trace, target));
                LogManager.Configuration = loggingConfig;

                var log = LogManager.GetCurrentClassLogger();
                AppDomain.CurrentDomain.UnhandledException += (sender, e) => log.Fatal((Exception)e.ExceptionObject);
                HostFactory.Run(x => x.Service<string>(o =>
                {
                    o.ConstructUsing(n => n);
                    o.WhenStarted(n => log.Info(Config.Global.Get<string>("greeting")));
                    o.WhenStopped(n => log.Info(Config.Global.Get<string>("valediction")));
                }));
            }
        }
    }
}
