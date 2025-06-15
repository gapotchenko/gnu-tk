// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Text;

namespace Gapotchenko.GnuTK.Toolkits.Wsl;

[SupportedOSPlatform("windows")]
static class PathUtil
{
    public static bool IsSubstitutedPath(string path)
    {
        string? driveLetter = Path.GetPathRoot(path)?.TrimEnd(Path.DirectorySeparatorChar);
        if (string.IsNullOrEmpty(driveLetter))
            return false;

        string? targetPath = TryQueryDosDevice(driveLetter);
        if (targetPath is null)
            return false;

        return targetPath.StartsWith(@"\??\", StringComparison.Ordinal);
    }

    static string? TryQueryDosDevice(string lpDeviceName)
    {
        var buffer = new StringBuilder(NativeMethods.MAX_PATH);

        for (; ; )
        {
            int result = NativeMethods.QueryDosDevice(lpDeviceName, buffer, buffer.Capacity);
            if (result == 0)
            {
                int error = Marshal.GetLastPInvokeError();
                if (error == NativeMethods.ERROR_INSUFFICIENT_BUFFER)
                    buffer.EnsureCapacity(buffer.Capacity * 2);
                else
                    return null;
            }
            else
            {
                buffer.Length = result;
                return buffer.ToString();
            }
        }
    }
}
