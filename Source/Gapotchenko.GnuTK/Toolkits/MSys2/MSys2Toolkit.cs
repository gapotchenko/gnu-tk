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
using Gapotchenko.GnuTK.Toolkits.Cygwin;
using Gapotchenko.Shields.MSys2.Deployment;
using System.Text;

namespace Gapotchenko.GnuTK.Toolkits.MSys2;

/// <summary>
/// Implements MSYS2 toolkit support.
/// </summary>
/// <remarks>
/// MSYS2 toolkit is built upon Cygwin.
/// </remarks>
sealed class MSys2Toolkit(MSys2ToolkitFamily family, IMSys2Environment msys2environment) :
    IScriptableToolkit,
    IToolkitEnvironment
{
    public string Name => field ??= $"{family.Name}-{msys2environment.Name}".ToLowerInvariant();

    public string Description => msys2environment.SetupInstance.DisplayName;

    public string? InstallationPath => msys2environment.SetupInstance.InstallationPath;

    public IToolkitFamily Family => family;

    public ToolkitIsolation Isolation => ToolkitIsolation.None;

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        // The 'sh' shell of MSYS2 is 'bash' in disguise.
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
        string shellPath = GetShellPath();

        var psi = new ProcessStartInfo
        {
            FileName = shellPath
        };

        var processEnvironment = psi.Environment;
        ToolkitEnvironment.CombineWith(processEnvironment, environment);
        ConfigureEnvironment(processEnvironment);
        ConfigureShellEnvironment(processEnvironment);

        // Launch the shell in POSIX mode to discourage usage of non-standard features.
        // This also makes a shell to start a lot faster.
        const bool posixifyShell = true;

        bool posixlyCorrect = processEnvironment.ContainsKey(ToolkitEnvironment.PosixlyCorrect);
        if (posixifyShell || posixlyCorrect)
        {
            // POSIX-compliant behavior requires us to add a lookup path for the shell directory.
            // Otherwise, the files in the shell directory cannot be found by the shell.

            List<string?> paths = [Path.GetDirectoryName(shellPath)];

            // Add MSYS2 environment paths as well.
            string environmentBinPath = Path.Combine(msys2environment.InstallationPath, "bin");
            if (Directory.Exists(environmentBinPath))
                paths.Add(environmentBinPath);

            ToolkitEnvironment.PrependPaths(processEnvironment, paths);
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

    string GetShellPath()
    {
        string shellPath = msys2environment.SetupInstance.ResolvePath(Path.Join("usr", "bin", "sh.exe"));
        if (!File.Exists(shellPath))
            throw new ProgramException(DiagnosticMessages.ModuleNotFound(shellPath));
        return shellPath;
    }

    public IReadOnlyDictionary<string, string?> Environment => field ??= GetEnvironmentCore();

    IReadOnlyDictionary<string, string?> GetEnvironmentCore()
    {
        var environment = ToolkitEnvironment.Create();
        ConfigureEnvironment(environment);

        string binPath = msys2environment.SetupInstance.ResolvePath(Path.Join("usr", "bin"));
        if (Directory.Exists(binPath))
            environment["PATH"] = binPath;

        return environment;
    }

    void ConfigureEnvironment(IDictionary<string, string?> environment)
    {
        string mSystem = msys2environment.Name;

        environment["MSYSTEM"] = mSystem;
        environment["MSYSCON"] = "";

        // Calculate a repository prefix to match package prefixes according to the repository selector:
        // https://packages.msys2.org/packages/
        string? repositoryPrefix =
            mSystem switch
            {
                "UCRT64" => "mingw-w64-ucrt-x86_64-",
                "CLANG64" => "mingw-w64-clang-x86_64-",
                "CLANGARM64" => "mingw-w64-clang-aarch64-",
                "MSYS" => "",
                "MINGW64" => "mingw-w64-x86_64-",
                "MINGW32" => "mingw-w64-i686-",
                _ => null
            };
        Debug.Assert(repositoryPrefix != null, "MSYS2 repository prefix calculation failed.");

        environment["GNU_TK_MSYS2_REPOSITORY_PREFIX"] = repositoryPrefix;
    }

    static void ConfigureShellEnvironment(IDictionary<string, string?> environment)
    {
        // MSYS2 will not do 'cd "${HOME}"' if environment variable 'CHERE_INVOKING' is defined.
        environment["CHERE_INVOKING"] = "1";

        // Make MSYS2 shell inherit 'PATH' environment variable from the host system.
        // In contrast to Cygwin, MSYS2 login shell does not inherit 'PATH' in non-POSIX mode.
        environment["MSYS2_PATH_TYPE"] = "inherit";
    }

    public string TranslateFilePath(string path) => CygwinFileSystem.TranslateFilePath(path, null);
}
