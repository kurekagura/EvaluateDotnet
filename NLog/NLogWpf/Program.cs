using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace NLogWpf;

public class Program
{
    [STAThread]
    public static void Main()
    {
        //var host = Host.CreateDefaultBuilder()
        //    .ConfigureServices(services =>
        //    {
        //        services.AddSingleton<App>();
        //        services.AddSingleton<MainWindow>();
        //    })
        //    .Build();
        //↑NLog追加前

        var hostBuilder = Host.CreateDefaultBuilder();
        hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
        {
            serviceCollection.AddSingleton<App>();
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(hostBuilderContext.Configuration); //Microsoft.Extensions.Configuration.ConfigurationRoot object
            });
        });

        var host = hostBuilder.Build(); //Build the host and get it!!
        var app = host.Services.GetService<App>();
        app!.Run();
    }
}
