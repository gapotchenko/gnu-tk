// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.IO;
using Gapotchenko.GnuTK.Diagnostics;
using Gapotchenko.Shields.Microsoft.Wsl.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.Wsl;

[SupportedOSPlatform("windows")]
sealed class WslToolkit(WslToolkitFamily family, IWslSetupInstance setupInstance) : IScriptableToolkit
{
    public string Name => "wsl";

    public string Description => setupInstance.DisplayName;

    public string? InstallationPath => setupInstance.InstallationPath;

    public IToolkitFamily Family => family;

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment)
    {
        return ExecuteCommand(
            "sh \"`wslpath \"$0\"`\" \"$@\"",
            [NormalizePath(path), .. arguments],
            environment);
    }

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment)
    {
        string shellPath = GetShellPath();

        var psi = new ProcessStartInfo
        {
            FileName = shellPath,
            WorkingDirectory = NormalizePath(Directory.GetCurrentDirectory())
        };

        if (environment != null)
            ToolkitKit.CombineEnvironmentWith(psi.Environment, environment);

        var args = psi.ArgumentList;
        args.Add("--exec");
        args.Add("sh");
        args.Add("-c");
        args.Add(command);
        args.AddRange(arguments);

        return ToolkitKit.ExecuteProcess(psi);
    }

    string GetShellPath()
    {
        string fileName = "wsl.exe";
        string shellPath = setupInstance.ResolvePath(Path.Join(fileName));
        if (!File.Exists(shellPath))
            throw new ProductException(DiagnosticMessages.ModuleNotFound(fileName));
        return shellPath;
    }

    static string NormalizePath(string path)
    {
        // WSL cannot map paths of substituted drives (as of v2.5.7.0).
        if (PathUtil.IsSubstitutedPath(path))
            return FileSystem.GetRealPath(path);
        else
            return path;
    }
}
