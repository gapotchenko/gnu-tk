// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.Shields.Cygwin.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.Cygwin;

sealed class CygwinToolkit(CygwinToolkitFamily family, ICygwinSetupInstance setupInstance) : IToolkit
{
    public string Name => "cygwin";

    public string Description => setupInstance.DisplayName;

    public string? InstallationPath => setupInstance.InstallationPath;

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
        string shellPath = setupInstance.ResolvePath(Path.Join("bin", "sh.exe"));
        if (!File.Exists(shellPath))
            throw new ProductException("Cannot find a Cygwin shell.");
        return shellPath;
    }

    static int ExecuteProcess(ProcessStartInfo psi)
    {
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        using var process =
            Process.Start(psi) ??
            throw new ProductException("Cygwin shell process cannot be started.");
        process.WaitForExit();
        return process.ExitCode;
    }
}
