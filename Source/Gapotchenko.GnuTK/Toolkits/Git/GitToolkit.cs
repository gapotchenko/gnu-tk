// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.GnuTK.Helpers;
using Gapotchenko.GnuTK.Toolkits.Cygwin;
using Gapotchenko.Shields.Git.Deployment;
using System.Text;

namespace Gapotchenko.GnuTK.Toolkits.Git;

sealed class GitToolkit(GitToolkitFamily family, IGitSetupInstance setupInstance, string shellPath) :
    IScriptableToolkit,
    IToolkitEnvironment
{
    public string Name => "git";

    public string Description => setupInstance.DisplayName;

    public string? InstallationPath => setupInstance.InstallationPath;

    public IToolkitFamily Family => family;

    public ToolkitTraits Traits => ToolkitTraits.None;

    public ToolkitIsolation Isolation => ToolkitIsolation.None;

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        // The 'sh' shell of the toolkit is 'bash' in disguise.
        return ExecuteCommandCore(command, arguments, environment, ["-e", "-o", "pipefail"]);
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return ExecuteCommandCore("sh \"$0\" \"$@\"", [path, .. arguments], environment, null);
    }

    int ExecuteCommandCore(
        string command,
        IReadOnlyList<string> commandArguments,
        IReadOnlyDictionary<string, string?>? environment,
        IEnumerable<string>? extraShellArguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = shellPath
        };

        var processEnvironment = psi.Environment;
        ToolkitEnvironment.CombineWith(processEnvironment, environment);

        // Launch the shell in POSIX mode to discourage usage of non-standard features.
        // This also makes a shell to start a lot faster.
        const bool posixifyShell = true;

        bool posixlyCorrect = processEnvironment.ContainsKey(ToolkitEnvironment.PosixlyCorrect);
        if (posixifyShell || posixlyCorrect)
        {
            // POSIX-compliant behavior requires us to add a lookup path for the shell directory.
            // Otherwise, the files in the shell directory cannot be found by the shell.
            ToolkitEnvironment.PrependPaths(processEnvironment, Path.GetDirectoryName(shellPath));
        }

        var shellArguments = psi.ArgumentList;
        if (posixifyShell && !posixlyCorrect)
            shellArguments.Add("-posix");
        shellArguments.Add("-l");
        if (extraShellArguments != null)
            shellArguments.AddRange(extraShellArguments);
        shellArguments.Add("-c");

        var commandBuilder = new StringBuilder();
        if (!posixlyCorrect)
        {
            // 'sh.exe' forcibly sets 'POSIXLY_CORRECT' environment variable.
            // Unset it if not instructed by a user.
            commandBuilder.Append("unset POSIXLY_CORRECT;");
        }
        commandBuilder.Append(command);
        shellArguments.Add(commandBuilder.ToString());

        if (commandArguments is [])
            shellArguments.Add(shellPath);
        else
            shellArguments.AddRange(commandArguments);

        return ProcessHelper.Execute(psi);
    }

    public IReadOnlyDictionary<string, string?> Environment => field ??= GetEnvironmentCore();

    IReadOnlyDictionary<string, string?> GetEnvironmentCore()
    {
        var environment = ToolkitEnvironment.Create();

        string binPath = setupInstance.ResolvePath(Path.Join("usr", "bin"));
        if (Directory.Exists(binPath))
            environment["PATH"] = binPath;

        return environment;
    }

    public string TranslateFilePath(string path) => CygwinFileSystem.TranslateFilePath(path, null);
}
