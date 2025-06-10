// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Hosting;

/// <summary>
/// Provides services for a host operating system.
/// </summary>
static class HostServices
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
                return RuntimeInformation.OSDescription;
        }
    }
}
