// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.Shields.Cygwin.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.Cygwin;

/// <summary>
/// Describes a family of GNU toolkits provided by Cygwin
/// Unix-like environment and command-line interface for Microsoft Windows.
/// </summary>
/// <remarks>
/// More information: <see href="https://cygwin.com/"/>
/// </remarks>
sealed class CygwinToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new CygwinToolkitFamily();

    CygwinToolkitFamily()
    {
    }

    public string Name => "Cygwin";

    public string? InformativeName => null;

    public IReadOnlyList<string> Aliases => [];

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.Installable | ToolkitFamilyTraits.Portable | ToolkitFamilyTraits.FilePathTranslation;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() =>
        CygwinDeployment.EnumerateSetupInstances()
        .Select(setupInstance => CreateToolkit(setupInstance, ToolkitTraits.None));

    public IEnumerable<IToolkit> EnumerateToolkitsInDirectory(string path, ToolkitTraits traits) =>
        CygwinSetupInstance.TryOpen(path) is { } setupInstance
            ? [CreateToolkit(setupInstance, traits)]
            : [];

    CygwinToolkit CreateToolkit(ICygwinSetupInstance setupInstance, ToolkitTraits traits) => new(this, setupInstance, traits);
}
