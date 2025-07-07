// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.IO;
using Gapotchenko.GnuTK.Diagnostics;
using Gapotchenko.GnuTK.Toolkits.System;
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
    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return ExecuteShell(["-c", command, .. arguments], environment, options);
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return ExecuteShell([path, .. arguments], environment, options);
    }

    int ExecuteShell(IEnumerable<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        string processPath = GetShellPath();

        var psi = new ProcessStartInfo
        {
            FileName = processPath
        };

        var processEnvironment = psi.Environment;
        ConfigureShellEnvironment(processEnvironment, options);
        EnvironmentServices.CombineEnvironmentWith(processEnvironment, environment);

        psi.ArgumentList.AddRange(arguments);

        return ToolkitKit.ExecuteProcess(psi);
    }

    void ConfigureShellEnvironment(
        IDictionary<string, string?> environment,
        ToolkitExecutionOptions options)
    {
        if ((options & ToolkitExecutionOptions.Strict) != 0 &&
            SystemToolkitFamily.Instance.Traits.HasFlag(ToolkitFamilyTraits.Alike))
        {
            // Remove lookup paths for tools provided by a GNU-like operating system
            // because they are not guaranteed to have strict GNU semantics.
            EnvironmentServices.RemovePath(environment, "/bin", "/usr/bin");
        }

        EnvironmentServices.CombineEnvironmentWith(environment, Environment);
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
