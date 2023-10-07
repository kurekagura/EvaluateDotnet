using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WpfApp.Services;
using WpfApp.ViewModels;

namespace WpfApp;

public class Program
{
    //public static IHost AppHost;

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

        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();
        //hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
        //{
        //    serviceCollection.AddSingleton<App>();
        //    serviceCollection.AddSingleton<MainWindow>();
        //    serviceCollection.AddLogging(loggingBuilder =>
        //    {
        //        // configure Logging with NLog
        //        loggingBuilder.ClearProviders();
        //        loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        //        loggingBuilder.AddNLog(hostBuilderContext.Configuration); //Microsoft.Extensions.Configuration.ConfigurationRoot object
        //    });
        //});
        //↑NLog追加例

        hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
        {
            serviceCollection.AddSingleton<App>();
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddSingleton<IDriveService, DriveService>();
            serviceCollection.AddTransient<DrivesViewModel>();
        });
        IHost host = hostBuilder.Build();
        //AppHost = hostBuilder.Build();
        App? app = host.Services.GetService<App>();
        app!.Run();
    }
}
