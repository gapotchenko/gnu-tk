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

    public ToolkitFamilyTraits Traits { get; } = GetTraitsCore();

    static ToolkitFamilyTraits GetTraitsCore()
    {
        var traits = ToolkitFamilyTraits.Installable | ToolkitFamilyTraits.Portable | ToolkitFamilyTraits.FilePathTranslation;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // No ready-to-use BusyBox for macOS is available.
            traits |= ToolkitFamilyTraits.Unofficial;
        }
        return traits;
    }

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() =>
        BusyBoxDeployment.EnumerateSetupInstances()
        .Take(1)
        .Select(setupInstance => CreateToolkit(setupInstance, ToolkitTraits.None));

    public IEnumerable<IToolkit> EnumerateToolkitsInDirectory(string path, ToolkitTraits traits) =>
        BusyBoxSetupInstance.TryOpen(path) is { } setupInstance
            ? [CreateToolkit(setupInstance, traits)]
            : [];

    BusyBoxToolkit CreateToolkit(IBusyBoxSetupInstance setupInstance, ToolkitTraits traits) => new(this, setupInstance, traits);
}
