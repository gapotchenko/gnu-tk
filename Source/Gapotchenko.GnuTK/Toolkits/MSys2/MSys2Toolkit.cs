// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.Shields.MSys2.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.MSys2;

sealed class MSys2Toolkit(MSys2ToolkitFamily family, IMSys2Environment environment) : IToolkit
{
    public string Name => field ??= $"{family.Name}-{environment.Name}".ToLowerInvariant();

    public string Description => environment.SetupInstance.DisplayName;

    public string? InstallationPath => environment.SetupInstance.InstallationPath;

    public IToolkitFamily Family => family;

    public int ExecuteFile(string path, IReadOnlyList<string> arguments)
    {
        return ExecuteCommand("sh \"$0\" \"$@\"", [path, .. arguments]);
    }

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments)
    {
        string shellPath = GetShellPath();

        var psi = new ProcessStartInfo
        {
            FileName = shellPath
        };

        var env = psi.Environment;
        env["MSYSCON"] = "";
        env["MSYSTEM"] = environment.Name;

        var args = psi.ArgumentList;
        args.Add("-l");
        args.Add("-c");
        args.Add("cd \"$0\" && BASH_ARGV0=$1 && shift;" + command);
        args.Add(Directory.GetCurrentDirectory());
        if (arguments is [])
            args.Add(shellPath);
        else
            args.AddRange(arguments);

        return ExecuteProcess(psi);
    }

    string GetShellPath()
    {
        string shellPath = environment.SetupInstance.ResolvePath(Path.Join("usr", "bin", "sh.exe"));
        if (!File.Exists(shellPath))
            throw new ProductException("Cannot find a MSYS2 shell.");
        return shellPath;
    }

    static int ExecuteProcess(ProcessStartInfo psi)
    {
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        using var process =
            Process.Start(psi) ??
            throw new ProductException("MSYS2 shell process cannot be started.");
        process.WaitForExit();
        return process.ExitCode;
    }
}
