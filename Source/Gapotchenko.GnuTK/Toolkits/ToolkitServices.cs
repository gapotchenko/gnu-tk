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
using Gapotchenko.GnuTK.Toolkits.Homebrew;
using Gapotchenko.GnuTK.Toolkits.MSys2;
using Gapotchenko.GnuTK.Toolkits.System;
using Gapotchenko.GnuTK.Toolkits.Wsl;

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Provides services for GNU toolkits.
/// </summary>
static class ToolkitServices
{
    /// <summary>
    /// Tries to get a scriptable toolkit.
    /// </summary>
    /// <param name="toolkits">The sequence of toolkits to get a scriptable toolkit from.</param>
    /// <returns>
    /// The scriptable toolkit
    /// or <see langword="null"/> if it cannot be obtained.
    /// </returns>
    public static IScriptableToolkit? TryGetScriptableToolkit(IEnumerable<IToolkit> toolkits)
    {
        IScriptableToolkit? scriptableToolkit = null;
        List<IToolkitEnvironment>? toolkitEnvironments = null;

        foreach (var toolkit in toolkits)
        {
            if (toolkit is IScriptableToolkit scriptableToolkitCandidate &&
                (toolkitEnvironments == null || scriptableToolkitCandidate.Isolation == ToolkitIsolation.None))
            {
                scriptableToolkit = scriptableToolkitCandidate;
                break;
            }

            if (toolkit is IToolkitEnvironment toolkitEnvironment)
                (toolkitEnvironments ??= []).Add(toolkitEnvironment);
        }

        if (scriptableToolkit is null || toolkitEnvironments is null)
            return scriptableToolkit;

        return new MetaToolkit(scriptableToolkit, toolkitEnvironments);
    }

    /// <summary>
    /// Selects toolkits according to the the specified names.
    /// </summary>
    /// <param name="toolkits">The sequence of toolkits to select from.</param>
    /// <param name="names">
    /// The names of selectable toolkits,
    /// or <see langword="null"/> to select a toolkit automatically.
    /// </param>
    /// <returns>The selected toolkits.</returns>
    public static IEnumerable<IToolkit> SelectToolkits(IEnumerable<IToolkit> toolkits, IEnumerable<string>? names)
    {
        // Step 1. If no specific names are specified, return all toolkits
        if (names is null)
            return toolkits;

        toolkits = toolkits.Memoize();

        // Step 2. Select toolkits by names
        List<IToolkit>? selectedToolkits = null;
        const StringComparison sc = StringComparison.OrdinalIgnoreCase;
        foreach (string name in names)
        {
            if (name.Equals("auto", sc))
                return selectedToolkits?.Union(toolkits) ?? toolkits;

            var selectedToolkit =
                toolkits.FirstOrDefault(toolkit => toolkit.Name.Equals(name, sc)) ?? // find by a precise toolkit name
                toolkits.FirstOrDefault(toolkit => toolkit.Family.Name.Equals(name, sc)); // otherwise, fallback to a toolkit family name

            if (selectedToolkit is not null)
                (selectedToolkits ??= []).Add(selectedToolkit);
        }

        // Step 3. To use a toolkit environment, at least one non-isolated scriptable toolkit is needed
        if (selectedToolkits != null && // if selected toolkits
            !selectedToolkits.OfType<IScriptableToolkit>().Any() && // have no scriptable toolkits
            selectedToolkits.OfType<IToolkitEnvironment>().Any()) // but have a toolkit environment
        {
            var scriptableToolkit = toolkits
                .OfType<IScriptableToolkit>()
                .FirstOrDefault(toolkit => toolkit.Isolation == ToolkitIsolation.None);

            if (scriptableToolkit != null)
                selectedToolkits.Add(scriptableToolkit);
        }

        return selectedToolkits ?? Enumerable.Empty<IToolkit>();
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

        // Portable toolkits are prioritized according to the paths' order.
        var portableToolkits = paths.SelectMany(path => families.SelectMany(family => family.EnumerateToolkitsFromDirectory(path)));

        // Installed toolkits are prioritized according to the families' order.
        var installedToolkits = families.SelectMany(family => family.EnumerateInstalledToolkits());

        return
            portableToolkits // portable toolkits have priority
            .Concat(installedToolkits) // installed toolkits come next            
            .DistinctBy( // avoid duplicates
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
            // Priority considerations:
            //   1. MSYS2 comes with a saner set of packages by default, easy mental model (install and forget)
            //   2. Cygwin provides better execution performance when compared to WSL,
            //      but mental model is on a heavier side (too customizable to the point of a possible frustration)
            //   3. WSL is ubiquitous and configurable, but it is prone to path mapping issues and to delays
            //      caused by VM spin ups.
            return [MSys2ToolkitFamily.Instance, CygwinToolkitFamily.Instance, WslToolkitFamily.Instance];
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // That was an easy one :)
            return [SystemToolkitFamily.Instance];

            // Homebrew package manager can be installed on Linux, but there
            // is no need to handle it specifically here because it
            // immersively integrates with a host system by itself.
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // While GNU is not Unix, macOS is a close enough Unix-based alternative.
            return [HomebrewToolkitFamily.Instance, SystemToolkitFamily.Instance];
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            // Generic fallback on a Unix-based host system.
            // While GNU is not Unix, Unix is a close enough native alternative.
            return [SystemToolkitFamily.Instance];
        }
        else
        {
            // Unsupported operating system.
            return [];
        }
    }
}
