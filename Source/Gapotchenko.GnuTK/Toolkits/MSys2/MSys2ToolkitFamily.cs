// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.Shields.MSys2.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.MSys2;

/// <summary>
/// Describes a family of GNU toolkits provided by MSYS2
/// software distribution and building platform for Microsoft Windows.
/// </summary>
/// <remarks>
/// More information: <see href="https://www.msys2.org/"/>
/// </remarks>
sealed class MSys2ToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new MSys2ToolkitFamily();

    MSys2ToolkitFamily()
    {
    }

    public string Name => "MSYS2";

    public IReadOnlyList<string> Aliases => ["MSYS"];

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.Installable | ToolkitFamilyTraits.Portable | ToolkitFamilyTraits.FilePathTranslation;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() =>
        MSys2Deployment.EnumerateSetupInstances()
        .SelectMany(setupInstance => EnumerateToolkits(setupInstance, ToolkitTraits.None));

    public IEnumerable<IToolkit> EnumerateToolkitsInDirectory(string path, ToolkitTraits traits) =>
        MSys2SetupInstance.TryOpen(path) is { } setupInstance
            ? EnumerateToolkits(setupInstance, traits)
            : [];

    IEnumerable<MSys2Toolkit> EnumerateToolkits(IMSys2SetupInstance setupInstance, ToolkitTraits traits) =>
        setupInstance.EnumerateEnvironments()
        .Select(environment => new MSys2Toolkit(this, environment, traits));
}
