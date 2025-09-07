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
using Gapotchenko.Shields.Microsoft.Wsl.Deployment;
using System.Text;

namespace Gapotchenko.GnuTK.Toolkits.Wsl;

[SupportedOSPlatform("windows")]
sealed class WslToolkit(WslToolkitFamily family, IWslSetupInstance setupInstance) : IScriptableToolkit
{
    public string Name => "wsl";

    public string Description => setupInstance.DisplayName;

    public string? InstallationPath => setupInstance.InstallationPath;

    public ToolkitTraits Traits => ToolkitTraits.None;

    public IToolkitFamily Family => family;

    public ToolkitIsolation Isolation =>
        setupInstance.Version.Major switch
        {
            >= 2 => ToolkitIsolation.VirtualMachine,
            _ => ToolkitIsolation.Container
        };

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        // 'exec' replaces the current shell process with the specified program or command.
        return ExecuteShellCommand(
            "exec \"$0\" \"$@\"",
            [path, .. arguments],
            environment,
            options);
    }

    public int ExecuteShellCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        return ExecuteShell(
            command,
            arguments,
            environment,
            ["-e"]);
    }

    public int ExecuteShellFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        return ExecuteShell(
            "exec sh \"$(wslpath \"$0\")\" \"$@\"",
            [NormalizePath(path), .. arguments],
            environment,
            null);
    }

    int ExecuteShell(
        string command,
        IReadOnlyList<string> commandArguments,
        IReadOnlyDictionary<string, string?> environment,
        IEnumerable<string>? extraShellArguments)
    {
        string wslPath = GetWslPath();

        var psi = new ProcessStartInfo
        {
            FileName = wslPath,
            WorkingDirectory = NormalizePath(Directory.GetCurrentDirectory())
        };

        var processEnvironment = psi.Environment;
        ToolkitEnvironment.CombineWith(processEnvironment, environment);

        var wslArguments = psi.ArgumentList;
        wslArguments.Add("--exec");
        wslArguments.Add("sh");
        wslArguments.Add("-l");
        if (extraShellArguments != null)
            wslArguments.AddRange(extraShellArguments);
        wslArguments.Add("-c");

        var commandBuilder = new StringBuilder();

        var environmentScriptWriter = new StringWriter();
        WriteEnvironmentScript(environmentScriptWriter, TranslateEnvironment(processEnvironment.AsReadOnly()));
        string environmentScript = environmentScriptWriter.ToString();

        if (environmentScript is not [])
            commandBuilder.Append(environmentScript).Append(';');

        commandBuilder.Append(command);
        wslArguments.Add(commandBuilder.ToString());

        wslArguments.AddRange(commandArguments);

        return ProcessHelper.Execute(psi);
    }

    string GetWslPath()
    {
        string fileName = "wsl.exe";
        string wslPath = setupInstance.ResolvePath(Path.Join(fileName));
        if (!File.Exists(wslPath))
            throw new ProgramException(DiagnosticMessages.ModuleNotFound(fileName));
        return wslPath;
    }

    static string NormalizePath(string path)
    {
        // WSL cannot map paths on substituted drives (as of v2.5.7.0).
        if (PathUtil.IsSubstitutedPath(path))
        {
            // Workaround that by explicitly mapping such a path to the real path.
            return FileSystem.GetRealPath(path);
        }
        else
        {
            return path;
        }
    }

    static void WriteEnvironmentScript(TextWriter writer, IReadOnlyDictionary<string, string?> environment)
    {
        bool first = true;

        foreach (var (name, value) in environment)
        {
            if (first)
                first = false;
            else
                writer.Write(';');

            if (value is null)
            {
                writer.Write("unset ");
                writer.Write(ShellHelper.Escape(name));
            }
            else
            {
                writer.Write("export ");
                writer.Write(ShellHelper.Escape(name));
                writer.Write('=');
                writer.Write(ShellHelper.Escape(value));
            }
        }
    }

    IReadOnlyDictionary<string, string?> TranslateEnvironment(IReadOnlyDictionary<string, string?> environment)
    {
        var newEnvironment = new Dictionary<string, string?>(StringComparer.Ordinal);

        Translate(ToolkitEnvironment.PosixlyCorrect);
        Translate("GNU_TK", true);
        Translate("GNU_TK_TOOLKIT");
        Translate("GNU_TK_VERSION");

        if (!newEnvironment.ContainsKey(ToolkitEnvironment.PosixlyCorrect))
            newEnvironment.Add(ToolkitEnvironment.PosixlyCorrect, null);

        return newEnvironment;

        void Translate(string name, bool translateFilePath = false)
        {
            if (environment.TryGetValue(name, out string? value))
            {
                if (value != null)
                {
                    if (translateFilePath)
                        value = TranslateFilePath(value);
                }
                newEnvironment[name] = value;
            }
        }
    }

    public string TranslateFilePath(string path)
    {
        if (path is [])
            return path;

        string wslPath = GetWslPath();

        var psi = new ProcessStartInfo
        {
            FileName = wslPath,
            WorkingDirectory = NormalizePath(Directory.GetCurrentDirectory())
        };

        var wslArguments = psi.ArgumentList;
        wslArguments.Add("--exec");
        wslArguments.Add("wslpath");
        wslArguments.Add(NormalizePath(path));

        var output = new StringWriter();

        int exitCode = ProcessHelper.Execute(psi, output);
        if (exitCode != 0)
            return path;

        return output.ToString();
    }
}
