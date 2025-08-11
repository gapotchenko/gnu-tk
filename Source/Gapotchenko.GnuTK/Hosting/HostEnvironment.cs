// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.GnuTK.IO;

namespace Gapotchenko.GnuTK.Hosting;

/// <summary>
/// Provides services for the host environment.
/// </summary>
static class HostEnvironment
{
    /// <summary>
    /// Gets a name of the current operating system.
    /// </summary>
    public static string OSName
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "Windows";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "Linux";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "macOS";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return "FreeBSD";
            else
                throw new PlatformNotSupportedException();
        }
    }

    /// <summary>
    /// Gets a file path format of the current operating system.
    /// </summary>
    public static FilePathFormat FilePathFormat => m_CachedFilePathFormat ??= GetFilePathFormatCore();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static FilePathFormat? m_CachedFilePathFormat;

    static FilePathFormat GetFilePathFormatCore() =>
        Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => FilePathFormat.Windows,
            PlatformID.Unix => FilePathFormat.Unix,
            _ => throw new PlatformNotSupportedException()
        };
}
