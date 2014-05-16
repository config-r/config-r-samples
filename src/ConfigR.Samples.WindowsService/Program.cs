// <copyright file="Program.cs" company="ConfigR contributors">
//  Copyright (c) ConfigR contributors. (configr.net@gmail.com)
// </copyright>

using System.ServiceProcess;

namespace ConfigR.Samples.WindowsService
{
    using System;
    using Common.Logging;
    using ConfigR;

    class Program : ServiceBase
    {
        static void Main()
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

        ILog log;

        protected override void OnStart(string[] args)
        {
            log = LogManager.GetCurrentClassLogger();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => log.Fatal((Exception)e.ExceptionObject);
            log.Info(Config.Global.Get<string>("greeting"));
        }

        protected override void OnStop()
        {
            log.Info(Config.Global.Get<string>("valediction"));
        }

    }
}
