using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp.Models;
using WpfApp.Services;
using SIO = System.IO;

namespace WpfApp.ViewModels;

public partial class DrivesViewModel : ObservableObject
{
    private IDriveService _driveSvc { get; }
    private IFileSystemService _fsSvc { get; }

    public DrivesViewModel(IDriveService driveService, IFileSystemService fileSystemService)
    {
        _driveSvc = driveService;
        _fsSvc = fileSystemService;
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
    public async Task SelectedDriverChanged(DriveInfo currentItem)
    {
        if (currentItem.DriveType != SIO::DriveType.Unknown)
        {
            var items = await _fsSvc.GetFileSystemAsync(currentItem.Letter);
        }
    }
}
