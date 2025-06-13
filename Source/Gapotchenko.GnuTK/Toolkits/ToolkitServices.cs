// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO;
using Gapotchenko.FX.Tuples;
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
    /// Selects the toolkit by the specified name.
    /// </summary>
    /// <param name="toolkits">The sequence of toolkits to select from.</param>
    /// <param name="name">
    /// The name of a toolkit to select,
    /// or <see langword="null"/> to select a toolkit automatically.
    /// </param>
    /// <returns>
    /// The selected toolkit
    /// or <see langword="null"/> if it cannot be selected.
    /// </returns>
    public static IToolkit? TrySelectToolkit(IEnumerable<IToolkit> toolkits, string? name) =>
        name is null
            ? toolkits.FirstOrDefault()
            : toolkits.FirstOrDefault(
                toolkit =>
                    toolkit.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    toolkit.Family.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Enumerates portable and installed toolkits.
    /// </summary>
    /// <param name="paths">The paths to portable toolkits.</param>
    /// <returns>A sequence of discovered toolkits.</returns>
    public static IEnumerable<IToolkit> EnumerateToolkits(IEnumerable<string> paths)
    {
        var families = SupportedToolkitFamilies;

        // Portable toolkits are prioritized according to the paths order.
        var portableToolkits = paths.SelectMany(path => families.SelectMany(family => family.EnumerateToolkitsFromDirectory(path)));

        // Installed toolkits are prioritized according to the families order.
        var installedToolkits = families.SelectMany(family => family.EnumerateInstalledToolkits());

        return
            // Portable toolkits have priority, return them first.
            portableToolkits
            .Concat(installedToolkits)
            // Avoid duplicates.
            .DistinctBy(
                toolkit => (toolkit.Name, toolkit.InstallationPath),
                ValueTupleEqualityComparer.Create(StringComparer.Ordinal, FileSystem.PathComparer));
    }

    /// <summary>
    /// Gets toolkit families supported on the current operating system.
    /// </summary>
    public static IReadOnlyList<IToolkitFamily> SupportedToolkitFamilies { get; } = GetSupportedToolkitFamilies();

    static IReadOnlyList<IToolkitFamily> GetSupportedToolkitFamilies()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Thoughts on priority:
            //   1. MSYS2 comes with most of the packages by default, easy mental model (install and forget)
            //   2. Cygwin provides better execution performance when compared to WSL,
            //      but mental model is on a heavy side (too customizable to the point of frustration)
            //   3. WSL is ubiquitous and configurable, but it is prone to path mapping issues
            return [MSys2ToolkitFamily.Instance, CygwinToolkitFamily.Instance];
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // That was an easy one :)
            return [NativeToolkitFamily.Instance];
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return [
                // https://formulae.brew.sh/formula/coreutils
                // https://uutils.github.io/coreutils/
                // https://www.topbug.net/blog/2013/04/14/install-and-use-gnu-command-line-tools-in-mac-os-x/
                //CoreUtilsToolkitFamily.Instance,
                // While GNU is not Unix, macOS is a close enough Unix-based native alternative.
                NativeToolkitFamily.Instance];
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            // Generic fallback.
            // While GNU is not Unix, Unix is a close enough native alternative.
            return [NativeToolkitFamily.Instance];
        }
        else
        {
            return [];
        }
    }
}
