// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.IO;
using Gapotchenko.GnuTK.Diagnostics;
using Gapotchenko.GnuTK.Helpers;
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
    public int ExecuteShellCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        // Bash and zsh both support 'pipefail' option.
        return ExecuteShell(
            ["-e", "-o", "pipefail", "-c", command, .. arguments],
            environment,
            options);
    }

    public int ExecuteShellFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        return ExecuteShell(
            [path, .. arguments],
            environment,
            options);
    }

    int ExecuteShell(IEnumerable<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        return ExecuteFileCore(GetShellPath(), arguments, environment, options);
    }

    string GetShellPath()
    {
        string packagePath = PackageManagement.GetPackagePath(shellPackage);
        string shellPath = Path.Combine(packagePath, "bin", shellPackage.Name);
        if (!File.Exists(shellPath))
            throw new ProgramException(DiagnosticMessages.ModuleNotFound(shellPath));
        return shellPath;
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        return ExecuteFileCore(path, arguments, environment, options);
    }

    int ExecuteFileCore(string path, IEnumerable<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        var psi = new ProcessStartInfo
        {
            FileName = path
        };

        var processEnvironment = psi.Environment;
        ConfigureEnvironment(processEnvironment, options);
        ToolkitEnvironment.CombineWith(processEnvironment, environment);

        psi.ArgumentList.AddRange(arguments);

        return ProcessHelper.Execute(psi);
    }

    void ConfigureEnvironment(
        IDictionary<string, string?> environment,
        ToolkitExecutionOptions options)
    {
        if ((options & ToolkitExecutionOptions.Strict) != 0 &&
            SystemToolkitFamily.Instance.Traits.HasFlag(ToolkitFamilyTraits.Alike))
        {
            // Remove lookup paths for tools provided by a GNU-like operating system
            // because they are not guaranteed to have strict GNU semantics.
            ToolkitEnvironment.RemovePaths(environment, "/bin", "/usr/bin");
        }

        ToolkitEnvironment.CombineWith(environment, Environment);
    }

    public string TranslateFilePath(string path) => path;
}
