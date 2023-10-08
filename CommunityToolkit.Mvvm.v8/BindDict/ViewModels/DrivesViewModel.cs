using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BindDict.ViewModels;

public partial class DrivesViewModel : ObservableObject
{
    public DrivesViewModel()
    {
    }

    //ポイント：Dictionary
    [ObservableProperty]
    private Dictionary<string, DriveInfo> _drives = new();

    [ObservableProperty]
    private int? _currentKey = null;

    //=> GetDriversCommand が生成される
    [RelayCommand]
    public void GetDrivers()
    {
        foreach (var di in DriveInfo.GetDrives())
        {
            Drives.Add(di.Name, di);
        }

        if (Drives.Count > 0)
            CurrentKey = 0;
    }

    [RelayCommand]
    public void SelectedDriverChanged(KeyValuePair<string, DriveInfo> currentItem)
    {
    }
}
