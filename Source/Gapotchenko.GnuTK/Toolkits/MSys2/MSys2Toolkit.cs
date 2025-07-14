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
using System.Text;

namespace Gapotchenko.GnuTK.Toolkits.MSys2;

sealed class MSys2Toolkit(MSys2ToolkitFamily family, IMSys2Environment msys2environment) :
    IScriptableToolkit,
    IToolkitEnvironment
{
    public string Name => field ??= $"{family.Name}-{msys2environment.Name}".ToLowerInvariant();

    public string Description => msys2environment.SetupInstance.DisplayName;

    public string? InstallationPath => msys2environment.SetupInstance.InstallationPath;

    public IToolkitFamily Family => family;

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return ExecuteCommand("sh \"$0\" \"$@\"", [path, .. arguments], environment, options);
    }

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        string processPath = GetShellPath();

        var psi = new ProcessStartInfo
        {
            FileName = processPath
        };

        var processEnvironment = psi.Environment;
        EnvironmentServices.CombineEnvironmentWith(processEnvironment, environment);
        SetEnvironment(processEnvironment);

        // Launch the shell in POSIX mode to discourage usage of non-standard features.
        // Another reason to run the MSYS2 shell in POSIX mode is to make it inherit
        // PATH environment variable from the host system. In contrast to Cygwin,
        // MSYS2 does not inherit PATH in non-POSIX mode.
        const bool posixifyShell = true;

        bool posixlyCorrect = processEnvironment.ContainsKey("POSIXLY_CORRECT");
        if (posixifyShell || posixlyCorrect)
        {
            // POSIX-compliant behavior requires us to add a lookup path for the shell directory.
            // Otherwise, the files in the shell directory cannot be found by the shell.
            EnvironmentServices.PrependPath(processEnvironment, Path.GetDirectoryName(processPath));
        }

        var processArguments = psi.ArgumentList;
        if (posixifyShell && !posixlyCorrect)
            processArguments.Add("-posix");
        processArguments.Add("-l");
        processArguments.Add("-c");

        var commandBuilder = new StringBuilder();
        commandBuilder.Append("cd \"$0\" && BASH_ARGV0=$1");
        if (!posixlyCorrect)
        {
            // 'sh.exe' forcibly sets POSIXLY_CORRECT environment variable.
            // Unset it if not instructed by a user.
            commandBuilder.Append(" && unset POSIXLY_CORRECT");
        }
        commandBuilder.Append(" && shift;").Append(command);
        processArguments.Add(commandBuilder.ToString());

        processArguments.Add(Directory.GetCurrentDirectory());
        if (arguments is [])
            processArguments.Add(processPath);
        else
            processArguments.AddRange(arguments);

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
        var environment = EnvironmentServices.CreateEnvironment();
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
