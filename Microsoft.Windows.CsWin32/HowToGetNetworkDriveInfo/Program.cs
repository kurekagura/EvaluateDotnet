namespace HowToGetNetworkDriveInfo;

/// <summary>
/// ネットワークドライブのUNC取得はWMIまたはWin32APIを利用する必要がある。
/// </summary>
internal class Program
{
    static void Main(string[] args)
    {
        var allDrives = DriveInfo.GetDrives();

        Console.WriteLine("ネットワークドライブ一覧:");
        foreach (DriveInfo drive in allDrives)
        {
            if (drive.DriveType == DriveType.Network)
            {
                Console.WriteLine($"ドライブ : {drive.Name}");
                var driveLetter = drive.Name.Trim('\\'); //X:\の\は渡してはNG。
                if (Win32Natives.TryWNetGetConnection(driveLetter, out string? unc))
                {
                    Console.WriteLine($"  UNC : {unc}");
                }
            }
        }
    }
}