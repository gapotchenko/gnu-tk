// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.Shields.MSys2.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.MSys2;

sealed class MSys2ToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new MSys2ToolkitFamily();

    MSys2ToolkitFamily()
    {
    }

    public string Name => "MSYS2";

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.Installable | ToolkitFamilyTraits.Portable;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() =>
        MSys2Deployment.EnumerateSetupInstances()
        .SelectMany(EnumerateToolkits);

    public IEnumerable<IToolkit> EnumerateToolkitsFromDirectory(string path) =>
        MSys2SetupInstance.TryOpen(path) is { } setupInstance
            ? EnumerateToolkits(setupInstance)
            : [];

    IEnumerable<MSys2Toolkit> EnumerateToolkits(IMSys2SetupInstance setupInstance) =>
        setupInstance.EnumerateEnvironments()
        .Select(environment => new MSys2Toolkit(this, environment));
}
