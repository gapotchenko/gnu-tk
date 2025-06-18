// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Tuples;
using Gapotchenko.GnuTK.Toolkits.Cygwin;
using Gapotchenko.GnuTK.Toolkits.MSys2;
using Gapotchenko.GnuTK.Toolkits.Native;
using Gapotchenko.GnuTK.Toolkits.Wsl;

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Provides services for GNU toolkits.
/// </summary>
static class ToolkitServices
{
    /// <summary>
    /// Selects the specified toolkits according to the the specified names.
    /// </summary>
    /// <param name="toolkits">The sequence of toolkits to select from.</param>
    /// <param name="names">
    /// The names of selectable toolkits,
    /// or <see langword="null"/> to select a toolkit automatically.
    /// </param>
    /// <returns>The selected toolkits.</returns>
    public static IEnumerable<IToolkit> SelectToolkits(IEnumerable<IToolkit> toolkits, IEnumerable<string>? names)
    {
        if (names is null)
            return toolkits;

        toolkits = toolkits.Memoize();
        IOrderedEnumerable<IToolkit>? selectedToolkits = null;
        const StringComparison sc = StringComparison.OrdinalIgnoreCase;

        foreach (string name in names)
        {
            if (name.Equals("auto", sc))
                return selectedToolkits?.Union(toolkits) ?? toolkits;

            selectedToolkits = selectedToolkits is null
                ? toolkits.OrderByDescending(GetOrderingKey)
                : selectedToolkits.ThenByDescending(GetOrderingKey);

            bool GetOrderingKey(IToolkit toolkit) => toolkit.Name.Equals(name, sc);
        }

        return selectedToolkits ?? toolkits;
    }

    /// <summary>
    /// Enumerates portable and installed toolkits.
    /// </summary>
    /// <param name="families">The sequence of toolkit families to take into consideration.</param>
    /// <param name="paths">The paths to portable toolkits.</param>
    /// <returns>A sequence of discovered toolkits.</returns>
    public static IEnumerable<IToolkit> EnumerateToolkits(IEnumerable<IToolkitFamily> families, IEnumerable<string> paths)
    {
        families = families.Memoize();

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
            //      but mental model is on a heavy side (too customizable to the point of a possible frustration)
            //   3. WSL is ubiquitous and configurable, but it is prone to path mapping issues (as of v2.5.7.0)
            return [MSys2ToolkitFamily.Instance, CygwinToolkitFamily.Instance, WslToolkitFamily.Instance];
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
