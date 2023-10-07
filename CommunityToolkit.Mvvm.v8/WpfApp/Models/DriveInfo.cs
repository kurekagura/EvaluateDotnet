namespace WpfApp.Models;

public class DriveInfo
{
    public required string Letter { get; set; }
    public System.IO.DriveType DriveType { get; set; }
}
