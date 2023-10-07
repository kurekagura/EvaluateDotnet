using System.Windows;

namespace WpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly MainWindow mainWindow;

    public App(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        mainWindow.Show();
        base.OnStartup(e);
    }
}
