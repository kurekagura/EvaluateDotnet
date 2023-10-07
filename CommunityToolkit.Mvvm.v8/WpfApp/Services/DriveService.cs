using System.Collections.Generic;
using System.Threading.Tasks;
using WpfApp.Models;

namespace WpfApp.Services;

public interface IDriveService
{
    Task<List<DriveInfo>> GetDrivesAsync();
}

public class DriveService : IDriveService
{
    public async Task<List<DriveInfo>> GetDrivesAsync()
    {
        var items = new List<DriveInfo>();
        items.Add(new DriveInfo() { Letter = "All Drives"});
        await Task.Run(() =>
        {
            foreach (System.IO.DriveInfo di in System.IO.DriveInfo.GetDrives())
            {
                items.Add(new DriveInfo { Letter = di.Name, DriveType = di.DriveType });
            }
        });

        return items;
    }
}
