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
        SetShellEnvironment(processEnvironment, options);
        EnvironmentServices.CombineEnvironmentWith(processEnvironment, environment);

        psi.ArgumentList.AddRange(arguments);

        return ToolkitKit.ExecuteProcess(psi);
    }

    void SetShellEnvironment(
        IDictionary<string, string?> environment,
        ToolkitExecutionOptions options)
    {
        if (SystemToolkitFamily.Instance.Traits.HasFlag(ToolkitFamilyTraits.Alike))
        {
            if ((options & ToolkitExecutionOptions.Strict) != 0)
            {
                if (environment.TryGetValue("PATH", out string? path) && path != null)
                {
                    // Remove tools provided by a GNU-like operating system from PATH.
                    environment["PATH"] = EnvironmentServices.JoinPath(
                        EnvironmentServices.SplitPath(path)
                        .Except(["/bin", "/usr/bin"], FileSystem.PathComparer));
                }
            }
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
