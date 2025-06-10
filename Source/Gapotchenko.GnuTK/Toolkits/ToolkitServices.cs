// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.GnuTK.Toolkits.Cygwin;
using Gapotchenko.GnuTK.Toolkits.MSys2;
using Gapotchenko.GnuTK.Toolkits.Native;

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Provides services for GNU toolkits.
/// </summary>
static class ToolkitServices
{
    /// <summary>
    /// Enumerates portable and installed toolkits.
    /// </summary>
    /// <param name="paths">The paths to portable toolkits.</param>
    /// <returns>A sequence of discovered toolkits.</returns>
    public static IEnumerable<IToolkit> EnumerateToolkits(IEnumerable<string> paths)
    {
        var families = ToolkitFamilies;

        // Portable toolkits are prioritized according to the paths order.
        var portableToolkits = paths.SelectMany(path => families.SelectMany(family => family.EnumerateToolkitsFromDirectory(path)));

        // Installed toolkits are prioritized according to the families order.
        var installedToolkits = families.SelectMany(family => family.EnumerateInstalledToolkits());

        return
            portableToolkits  // portable toolkits have priority, return them first
            .Concat(installedToolkits);
    }

    /// <summary>
    /// Gets toolkit families supported on the current operating system.
    /// </summary>
    public static IReadOnlyList<IToolkitFamily> ToolkitFamilies { get; } = GetToolkitFamilies();

    static IReadOnlyList<IToolkitFamily> GetToolkitFamilies()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return [MSys2ToolkitFamily.Instance, CygwinToolkitFamily.Instance];
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return [NativeToolkitFamily.Instance];
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // While GNU is not Unix, macOS is a close enough alternative.
            return [NativeToolkitFamily.Instance];
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            // Generic fallback.
            // While GNU is not Unix, Unix is a close enough alternative.
            return [NativeToolkitFamily.Instance];
        }
        else
        {
            return [];
        }
    }
}
