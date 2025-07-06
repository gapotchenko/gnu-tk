// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.Shields.Homebrew.Management;

namespace Gapotchenko.GnuTK.Toolkits.Homebrew;

sealed class HomebrewScriptableToolkit(
    HomebrewToolkitFamily family,
    IBrewPackageManagement packageManagement,
    IEnumerable<BrewPackage> packages,
    BrewPackage shellPackage) :
    HomebrewToolkitEnvironment(family, packageManagement, packages),
    IScriptableToolkit
{
    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment)
    {
        throw new NotImplementedException();
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment)
    {
        throw new NotImplementedException();
    }
}
