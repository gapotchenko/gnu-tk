// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.IO;
using Gapotchenko.GnuTK.Diagnostics;
using Gapotchenko.Shields.Cygwin.Deployment;
using System.Text;

namespace Gapotchenko.GnuTK.Toolkits.Cygwin;

sealed class CygwinToolkit(CygwinToolkitFamily family, ICygwinSetupInstance setupInstance) :
    IScriptableToolkit,
    IToolkitEnvironment
{
    public string Name => "cygwin";

    public string Description => setupInstance.DisplayName;

    public string? InstallationPath => setupInstance.InstallationPath;

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
        ConfigureShellEnvironment(processEnvironment);

        // Launch the shell in POSIX mode to discourage usage of non-standard features.
        // This also makes a shell start a lot faster.
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
        if (!posixlyCorrect)
        {
            // 'sh.exe' forcibly sets POSIXLY_CORRECT environment variable.
            // Unset it if not instructed by a user.
            commandBuilder.Append("unset POSIXLY_CORRECT;");
        }
        commandBuilder.Append(command);
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
        string shellPath = setupInstance.ResolvePath(Path.Join("bin", "sh.exe"));
        if (!File.Exists(shellPath))
            throw new ProgramException(DiagnosticMessages.ModuleNotFound(shellPath));
        return shellPath;
    }

    public IReadOnlyDictionary<string, string?> Environment => field ??= GetEnvironmentCore();

    IReadOnlyDictionary<string, string?> GetEnvironmentCore()
    {
        var environment = EnvironmentServices.CreateEnvironment();

        string binPath = setupInstance.ResolvePath("bin");
        if (Directory.Exists(binPath))
            environment["PATH"] = binPath;

        return environment;
    }

    static void ConfigureShellEnvironment(IDictionary<string, string?> environment)
    {
        // Cygwin will not do 'cd ${HOME}' if environment variable CHERE_INVOKING is defined.
        environment["CHERE_INVOKING"] = "1";
    }

    public string TranslateFilePath(string path) => CygwinFileSystem.TranslateFilePath(path, "/cygdrive");

    public ToolkitIsolation Isolation => ToolkitIsolation.None;
}
