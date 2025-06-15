using System.Text;

namespace Gapotchenko.GnuTK.Toolkits.Wsl;

[SupportedOSPlatform("windows")]
static class NativeMethods
{
    public const int MAX_PATH = 260;

    public const int ERROR_INSUFFICIENT_BUFFER = 122;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);
}
