// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.GnuTK.Diagnostics;
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
        return ExecuteShell(["-c", command, .. arguments], environment);
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment)
    {
        return ExecuteShell([path, .. arguments], environment);
    }

    int ExecuteShell(IEnumerable<string> arguments, IReadOnlyDictionary<string, string?>? environment)
    {
        string envPath = GetShellPath();

        var psi = new ProcessStartInfo
        {
            FileName = envPath
        };

        ToolkitKit.CombineEnvironmentWith(
            psi.Environment,
            ToolkitKit.CombineEnvironments(Environment, environment));

        var args = psi.ArgumentList;
        args.AddRange(arguments);

        return ToolkitKit.ExecuteProcess(psi);
    }

    string GetShellPath()
    {
        string packagePath = PackageManagement.GetPackagePath(shellPackage);
        string shellPath = Path.Combine(packagePath, "bin", shellPackage.Name);
        if (!File.Exists(shellPath))
            throw new ProductException(DiagnosticMessages.ModuleNotFound(shellPath));
        return shellPath;
    }
}
