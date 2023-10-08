using System.Windows;
using BindDict.ViewModels;

namespace BindDict;

public partial class MainWindow : Window
{
    public MainWindow(DrivesViewModel driveViewModel)
    {
        InitializeComponent();
        DataContext = driveViewModel;
    }
}
