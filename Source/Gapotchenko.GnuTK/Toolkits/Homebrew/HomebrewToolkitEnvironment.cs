// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Linq;
using Gapotchenko.Shields.Homebrew.Management;

namespace Gapotchenko.GnuTK.Toolkits.Homebrew;

class HomebrewToolkitEnvironment(
    HomebrewToolkitFamily family,
    IBrewPackageManagement packageManagement,
    IEnumerable<BrewPackage> packages) :
    IToolkitEnvironment
{
    public string Name => "brew";

    public string Description => "Homebrew package manager.";

    public string? InstallationPath => packageManagement.Manager.Setup.InstallationPath;

    public IReadOnlyDictionary<string, string?>? Environment => field ??= GetEnvironmentCore();

    IReadOnlyDictionary<string, string?> GetEnvironmentCore()
    {
        var env = ToolkitKit.CreateEnvironment();

        return env;
    }

    public IToolkitFamily Family => family;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly IEnumerable<BrewPackage> m_Packages = packages.Memoize();
}
