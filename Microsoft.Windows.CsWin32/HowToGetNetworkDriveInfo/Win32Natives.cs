using System.Diagnostics.CodeAnalysis;

namespace HowToGetNetworkDriveInfo;

public static class Win32Natives
{
    public static unsafe bool TryWNetGetConnection(string driveLetter, [MaybeNullWhen(false)] out string result, int bufferSize = 1024)
    {
        // CS8625 - Cannot convert null literal to non-nullable reference type.
        result = default;
        try
        {
            // pointer to a zero-terminated string
            Span<char> pszRemoteName = stackalloc char[bufferSize];

            // pointer to count of characters
            uint pcchLength = (uint)pszRemoteName.Length;

            fixed (char* pszOutLocal = pszRemoteName)
            {
                var ret = Windows.Win32.PInvoke.WNetGetConnection(driveLetter, new Windows.Win32.Foundation.PWSTR(pszOutLocal), ref pcchLength);
                if (ret != 0)
                    return false;

                // 確保領域の末尾までが\0で埋められているので削除。
                result = new string(pszRemoteName.TrimEnd('\0'));
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}
