using System.Windows;
using BindDict.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BindDict;

/// <summary>
/// DI構成をProgramでやらず、Appで行うパターン。
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        //    mainWindow.Show();
        base.OnStartup(e);
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.ConfigureServices((hostBuilderContext, serviceCollection) =>
        {
            serviceCollection.AddSingleton<MainWindow>();
            serviceCollection.AddTransient<DrivesViewModel>();
        });
        IHost host = hostBuilder.Build();
        MainWindow? window = host.Services.GetService<MainWindow>();
        window?.Show();
    }
}
