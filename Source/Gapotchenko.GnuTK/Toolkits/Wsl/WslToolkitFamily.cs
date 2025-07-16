// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Math.Intervals;
using Gapotchenko.Shields.Microsoft.Wsl.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.Wsl;

/// <summary>
/// Describes a family of GNU toolkits provided by Windows Subsystem for Linux.
/// </summary>
[SupportedOSPlatform("windows")]
sealed class WslToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new WslToolkitFamily();

    WslToolkitFamily()
    {
    }

    public string Name => "WSL";

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.Installable | ToolkitFamilyTraits.FilePathTranslation;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() =>
        WslDeployment.EnumerateSetupInstances(ValueInterval.FromInclusive(new Version(2, 0)))
        .Select(CreateToolkit);

    WslToolkit CreateToolkit(IWslSetupInstance setupInstance) => new(this, setupInstance);

    public IEnumerable<IToolkit> EnumerateToolkitsFromDirectory(string path) => [];
}
