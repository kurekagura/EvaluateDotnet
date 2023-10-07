using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp.Services;

namespace WpfApp.ViewModels;

public partial class DrivesViewModel : ObservableObject
{
    private IDriveService _driveSvc { get; }

    public DrivesViewModel(IDriveService driveService)
    {
        _driveSvc = driveService;
    }

    //バッキングフィールド => Drivers が生成される
    //オブジェクト参照を設定しておく必要がある
    [ObservableProperty]
    private System.Collections.Generic.List<WpfApp.Models.DriveInfo> _drives = new();

    [ObservableProperty]
    private int? _currentIndex = null;

    //=> GetDriversCommand が生成される
    [RelayCommand]
    public async Task GetDrivers()
    {
        foreach (Models.DriveInfo di in await _driveSvc.GetDrivesAsync())
        {
            Drives.Add(di);
        }
        if (Drives.Count > 0)
            CurrentIndex = 0;
    }

    [RelayCommand]
    public void SelectedDriverChanged()
    {
    }
}
