// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.Shields.Cygwin.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.Cygwin;

sealed class CygwinToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new CygwinToolkitFamily();

    CygwinToolkitFamily()
    {
    }

    public string Name => "Cygwin";

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.Installable | ToolkitFamilyTraits.Portable;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() =>
        CygwinDeployment.EnumerateSetupInstances()
        .Select(CreateToolkit);

    public IEnumerable<IToolkit> EnumerateToolkitsFromDirectory(string path) =>
        CygwinSetupInstance.TryOpen(path) is { } setupInstance
            ? [CreateToolkit(setupInstance)]
            : [];

    CygwinToolkit CreateToolkit(ICygwinSetupInstance setupInstance) => new(this, setupInstance);
}
