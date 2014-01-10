// <copyright file="IndexModule.cs" company="ConfigR contributors">
//  Copyright (c) ConfigR contributors. (configr.net@gmail.com)
// </copyright>

namespace ConfigR.Samples.WebApplication
{
    using System;
    using System.Globalization;
    using ConfigR;
    using Nancy;

    [CLSCompliant(false)]
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            this.Get["/"] = parameters =>
            {
                // NOTE (adamralph): in a real world app you probably wouldn't use configuration
                // directly within an HTTP module in this way.
                // In a real world app you'd typically use configuration to configure your IoC container.
                // In the case of a Nancy app, this would be done in your custom bootstrapper.
                var greeting = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}, I'm built for {1}!",
                    Config.Global.Get<string>("greeting"),
                    Config.Global.Get<string>("builtfor"));

                var model = new { Greeting = greeting };
                return View["index", model];
            };
        }
    }
}