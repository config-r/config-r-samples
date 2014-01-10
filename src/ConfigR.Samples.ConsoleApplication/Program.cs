// <copyright file="Program.cs" company="ConfigR contributors">
//  Copyright (c) ConfigR contributors. (configr.net@gmail.com)
// </copyright>

namespace ConfigR.Samples.ConsoleApplication
{
    using System;
    using System.IO;
    using Common.Logging;
    using Common.Logging.Simple;
    using ConfigR;
    using ServiceStack.Text;

    public static class Program
    {
        public static void Main(string[] args)
        {
            LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter(LogLevel.Debug, false, true, true, null);

            // you can retreive settings as their underlying type
            var count = Config.Global.Get<int>("Count");
            var uri = Config.Global.Get<Uri>("Uri");

            Console.WriteLine("Count: {0}", count);
            Console.WriteLine("Uri: {0}", uri);

            // reset to original state for the samples below
            Config.Global.Unload();

            // you can also have cascading configuration using multiple files
            Config.Global.LoadScriptFile("Custom1.csx").LoadScriptFile("Custom2.csx"); // Custom2.csx uses #load to get its data and does a nested load on Custom3.csx!
            count = Config.Global.Get<int>("Count");
            uri = Config.Global.Get<Uri>("Uri");
            var fromCustom1File = Config.Global.Get<bool>("FromCustom1File");
            var fromCustom2File = Config.Global.Get<bool>("FromCustom2File");
            var fromCustom3File = Config.Global.Get<bool>("FromCustom3File");

            Console.WriteLine("Count: {0}", count);                     // this still comes from the first file (local "ConfigR.Sample.exe.csx")
            Console.WriteLine("Uri: {0}", uri);                         // this still comes from the first file (local "ConfigR.Sample.exe.csx")
            Console.WriteLine("FromCustom1File: {0}", fromCustom1File); // this comes from the second file ("Custom1.csx")
            Console.WriteLine("FromCustom2File: {0}", fromCustom2File); // this comes from the third file ("Custom2.csx"), defined by "Custom2.Data.csx"
            Console.WriteLine("FromCustom3File: {0}", fromCustom3File); // this comes from the fourth file ("Custom3.csx")

            // you can even use config located on the web!
            Config.Global.LoadWebScript(new Uri("https://gist.github.com/adamralph/6843899/raw/8cfdb09ad00655edb389cb0761aca44fb24f83fb/sample-config2.csx"));
            Console.WriteLine("web-greeting: {0}", Config.Global.Get<string>("web-greeting"));

            // for completeness you can also use a file URI (or an FTP URI although that's not easily demonstrable)
            Config.Global.LoadWebScript(new Uri(Path.GetFullPath("Custom1.csx")));
            Console.WriteLine("FromCustom1File: {0}", Config.Global.Get<bool>("FromCustom1File"));

            // reset to original state for the samples below
            Config.Global.Unload();

            // your configuration script can also use types declared in your application
            Config.Global.LoadScriptFile("Custom4.csx");
            Console.WriteLine("Foo: {0}", Config.Global.Get<Foo>().ToJsv());

            // reset to original state for the samples below
            Config.Global.Unload();

            // you can pass values from your app to your config scripts
            Config.Global.Add("Foo", 123);
            Config.Global.LoadScriptFile("Custom5.csx");
            Console.WriteLine("Foo: {0}", Config.Global.Get<int>("Foo"));
            Console.WriteLine("Bar: {0}", Config.Global.Get<int>("Bar"));

            Console.WriteLine("Brutalize a key with your favourite finger to exit.");
            Console.ReadKey();
        }
    }
}
