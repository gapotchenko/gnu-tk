// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Linq;
using Gapotchenko.Shields.Homebrew.Deployment;
using Gapotchenko.Shields.Homebrew.Management;

namespace Gapotchenko.GnuTK.Toolkits.Homebrew;

/// <summary>
/// Describes a family of GNU toolkits provided by Homebrew package manager.
/// </summary>
/// <remarks>
/// More information: <see href="https://brew.sh/"/>
/// </remarks>
sealed class HomebrewToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new HomebrewToolkitFamily();

    public string Name => "Homebrew";

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.Installable;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() =>
        BrewDeployment.EnumerateSetupInstances()
        .SelectMany(EnumerateToolkits);

    public IEnumerable<IToolkit> EnumerateToolkitsInDirectory(string path, ToolkitTraits traits) => [];

    IEnumerable<HomebrewToolkitEnvironment> EnumerateToolkits(IBrewSetupInstance setupInstance)
    {
        var manager = BrewManagement.CreateManager(setupInstance);
        var formulae = manager.Formulae;

        IEnumerable<BrewPackage> GetPackages(IEnumerable<string> packageNames) =>
            packageNames.SelectMany(packageName => formulae.EnumeratePackages(packageName, BrewPackageEnumerationOptions.Current));

        var shellPackages = GetPackages(m_ShellPackageNames).Memoize();
        var gnuPackages = GetPackages(m_GnuPackageNames);

        if (shellPackages.Any())
        {
            yield return new HomebrewScriptableToolkit(
                this,
                formulae,
                gnuPackages.Concat(shellPackages),
                shellPackages.First());
        }
        else
        {
            yield return new HomebrewToolkitEnvironment(this, formulae, gnuPackages);
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static readonly string[] m_ShellPackageNames = ["bash", "zsh"];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    static readonly string[] m_GnuPackageNames =
    [
        "binutils", "coreutils", "findutils",
        "gnu-sed"
    ];
}
