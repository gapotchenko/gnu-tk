// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO;
using Gapotchenko.FX.Linq;
using Gapotchenko.GnuTK.Configuration;
using Gapotchenko.GnuTK.Diagnostics;
using Gapotchenko.GnuTK.Toolkits.BusyBox;
using Gapotchenko.GnuTK.Toolkits.Cygwin;
using Gapotchenko.GnuTK.Toolkits.Git;
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
                (toolkitEnvironments is null || scriptableToolkitCandidate.Isolation is ToolkitIsolation.None))
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
        var tnc = StringComparer.OrdinalIgnoreCase;
        foreach (string name in names)
        {
            if (tnc.Equals(name, "auto"))
                return selectedToolkits?.Union(toolkits) ?? toolkits;

            var selectedToolkit =
                toolkits.FirstOrDefault(toolkit => tnc.Equals(toolkit.Name, name)) ?? // find by a precise toolkit name
                toolkits.FirstOrDefault(toolkit => GetEffectiveToolkitAliases(toolkit).Contains(name, tnc)) ?? // otherwise, fallback to a toolkit alias name
                toolkits.FirstOrDefault(toolkit => tnc.Equals(toolkit.Family.Name, name)) ?? // otherwise, fallback to a toolkit family name
                toolkits.FirstOrDefault(toolkit => toolkit.Family.Aliases.Contains(name, tnc)); // otherwise, fallback to a toolkit family alias name

            if (selectedToolkit is not null)
                (selectedToolkits ??= []).Add(selectedToolkit);
        }

        // Step 3. To use a toolkit environment, at least one non-isolated scriptable toolkit is needed
        if (selectedToolkits is not null && // if the selected toolkits
            !selectedToolkits.OfType<IScriptableToolkit>().Any() && // have no scriptable toolkits
            selectedToolkits.OfType<IToolkitEnvironment>().Any()) // but have a toolkit environment
        {
            // Out of desperation, get a suitable scriptable toolkit from the common pool.
            var scriptableToolkit = toolkits
                .OfType<IScriptableToolkit>()
                .FirstOrDefault(toolkit => toolkit.Isolation is ToolkitIsolation.None);

            if (scriptableToolkit is not null)
                selectedToolkits.Add(scriptableToolkit);
        }

        return selectedToolkits ?? [];
    }

    static IEnumerable<string> GetEffectiveToolkitAliases(IToolkit toolkit)
    {
        if ((toolkit.Traits & ToolkitTraits.BuiltIn) != 0)
            return ["built-in", "builtin"];
        else
            return [];
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
        var portableToolkits = paths.SelectMany(path => families.SelectMany(family => family.EnumerateToolkitsInDirectory(path, ToolkitTraits.None)));

        // Installed toolkits are prioritized according to the families' order.
        var installedToolkits = families.SelectMany(family => family.EnumerateInstalledToolkits());

        // Built-in toolkits are prioritized according to the families' order.
        var builtInToolkits = families.SelectMany(EnumerateBuiltInToolkits);

        return
            portableToolkits // portable toolkits come first
            .Concat(installedToolkits) // installed toolkits come next            
            .Concat(builtInToolkits) // built-in toolkits come last
            .DistinctBy(toolkit => toolkit.Name, StringComparer.OrdinalIgnoreCase); // without duplicates
    }

    static IEnumerable<IToolkit> EnumerateBuiltInToolkits(IToolkitFamily family)
    {
        string name = family.Name;

        string? path = ConfigurationServices.TryGetSetting("Toolkits:BuiltIn:" + name);
        if (path is null)
            return [];

        string originalPath = path;
        path = Path.GetFullPath(path, ConfigurationServices.BaseDirectory);
        if (!Directory.Exists(path))
        {
            throw new DiagnosticException(
                DiagnosticMessages.BuiltInToolkitDirectoryNotFound(name, originalPath),
                DiagnosticCode.BuiltInToolkitDirectoryNotFound);
        }

        string ridPath = Path.Combine(path, RuntimeInformation.RuntimeIdentifier);
        if (Directory.Exists(ridPath))
            path = ridPath;

        return family.EnumerateToolkitsInDirectory(path, ToolkitTraits.BuiltIn);
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
            //   3. Git for Windows is not a GNU toolkit per se, but it comes with a built-in one based on Cygwin.
            //      Rigid but ubiquitous, often present in continuous integration systems by default
            //   4. WSL is ubiquitous and configurable, but prone to path mapping issues and to delays caused
            //      by VM's spin ups and spin downs
            //   5. BusyBox is a minimal GNU-like toolkit which is better than nothing when no specialized GNU
            //      toolkit is available
            return
                [
                    MSys2ToolkitFamily.Instance,
                    CygwinToolkitFamily.Instance,
                    GitToolkitFamily.Instance,
                    WslToolkitFamily.Instance,
                    BusyBoxToolkitFamily.Instance
                ];
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // While GNU is not Unix, macOS is a close enough Unix-based alternative.
            return
                [
                    HomebrewToolkitFamily.Instance,
                    SystemToolkitFamily.Instance,
                    // There is no ready-to-use BusyBox for macOS available, but we are be optimistic for the future.
                    BusyBoxToolkitFamily.Instance
                ];
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // That was an easy one :)
            return [SystemToolkitFamily.Instance, BusyBoxToolkitFamily.Instance];

            // Homebrew package manager can be installed on Linux,
            // but there is no need to handle it specifically here
            // because it immersively integrates with the host system by itself.
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            return [SystemToolkitFamily.Instance, BusyBoxToolkitFamily.Instance];
        }
        // Grey area: something we do not precisely know about.
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            // Generic fallback on a Unix-based host system.
            // While GNU is not Unix, Unix is a close enough alternative.
            return [SystemToolkitFamily.Instance, BusyBoxToolkitFamily.Instance];
        }
        else
        {
            // Unknown operating system.
            return [BusyBoxToolkitFamily.Instance];
        }
    }
}
