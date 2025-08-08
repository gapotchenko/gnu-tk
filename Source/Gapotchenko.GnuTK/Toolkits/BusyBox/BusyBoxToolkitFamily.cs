// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.Shields.BusyBox.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.BusyBox;

/// <summary>
/// Describes a family of BusyBox toolkits that combine tiny versions of many common
/// UNIX utilities into a single small executable.
/// BusyBox provides replacements for most of the utilities you usually find in GNU fileutils, shellutils, etc.
/// </summary>
/// <remarks>
/// More information:
/// <list type="bullet">
/// <item><see href="https://busybox.net/"/></item>
/// <item><see href="https://frippery.org/busybox/"/></item> (BusyBox for Windows)
/// </list>
/// </remarks>
sealed class BusyBoxToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new BusyBoxToolkitFamily();

    BusyBoxToolkitFamily()
    {
    }

    public string Name => "BusyBox";

    public IReadOnlyList<string> Aliases => [];

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.Installable | ToolkitFamilyTraits.Portable | ToolkitFamilyTraits.FilePathTranslation;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits()
    {
        IEnumerable<(IBusyBoxSetupInstance SetupInstance, ToolkitTraits Traits)> query = [];

        query = query
            .Concat(BusyBoxDeployment.EnumerateSetupInstances()
                .Take(1)
                .Select(setupInstance => (SetupInstance: setupInstance, ToolkitTraits.None)))
            .OrderByDescending(x => x.SetupInstance.Version);

        return query.Select(x => CreateToolkit(x.SetupInstance, x.Traits));
    }

    public IEnumerable<IToolkit> EnumerateToolkitsInDirectory(string path) =>
        EnumerateSetupInstancesInDirectory(path)
        .Select(setupInstance => CreateToolkit(setupInstance, ToolkitTraits.None));

    static IEnumerable<IBusyBoxSetupInstance> EnumerateSetupInstancesInDirectory(string path) =>
        BusyBoxSetupInstance.TryOpen(path) is { } setupInstance
            ? [setupInstance]
            : [];

    BusyBoxToolkit CreateToolkit(IBusyBoxSetupInstance setupInstance, ToolkitTraits traits) => new(this, setupInstance, traits);
}
