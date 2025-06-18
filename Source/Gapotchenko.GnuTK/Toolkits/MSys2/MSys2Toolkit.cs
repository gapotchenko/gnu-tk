// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.IO;
using Gapotchenko.GnuTK.Diagnostics;
using Gapotchenko.Shields.MSys2.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.MSys2;

sealed class MSys2Toolkit(MSys2ToolkitFamily family, IMSys2Environment msys2environment) : IScriptableToolkit, IToolkitEnvironment
{
    public string Name => field ??= $"{family.Name}-{msys2environment.Name}".ToLowerInvariant();

    public string Description => msys2environment.SetupInstance.DisplayName;

    public string? InstallationPath => msys2environment.SetupInstance.InstallationPath;

    public IToolkitFamily Family => family;

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment)
    {
        return ExecuteCommand("sh \"$0\" \"$@\"", [path, .. arguments], environment);
    }

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment)
    {
        string shellPath = GetShellPath();

        var psi = new ProcessStartInfo
        {
            FileName = shellPath
        };

        var env = psi.Environment;
        ToolkitKit.CombineEnvironmentWith(env, environment);
        SetEnvironment(env);

        var args = psi.ArgumentList;
        args.Add("-l");
        args.Add("-c");
        args.Add("cd \"$0\" && BASH_ARGV0=$1 && shift;" + command);
        args.Add(Directory.GetCurrentDirectory());
        if (arguments is [])
            args.Add(shellPath);
        else
            args.AddRange(arguments);

        return ToolkitKit.ExecuteProcess(psi);
    }

    string GetShellPath()
    {
        string shellPath = msys2environment.SetupInstance.ResolvePath(Path.Join("usr", "bin", "sh.exe"));
        if (!File.Exists(shellPath))
            throw new ProductException(DiagnosticMessages.ModuleNotFound(shellPath));
        return shellPath;
    }

    public IReadOnlyDictionary<string, string?> Environment => field ??= GetEnvironmentCore();

    IReadOnlyDictionary<string, string?> GetEnvironmentCore()
    {
        var environment = ToolkitKit.CreateEnvironment();
        SetEnvironment(environment);

        string binPath = msys2environment.SetupInstance.ResolvePath(Path.Join("usr", "bin"));
        if (Directory.Exists(binPath))
            environment["PATH"] = binPath;

        return environment;
    }

    void SetEnvironment(IDictionary<string, string?> environment)
    {
        environment["MSYSCON"] = "";
        environment["MSYSTEM"] = msys2environment.Name;
    }
}
