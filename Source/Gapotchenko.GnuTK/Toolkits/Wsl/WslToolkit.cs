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
using System.Text;

namespace Gapotchenko.GnuTK.Toolkits.Wsl;

[SupportedOSPlatform("windows")]
sealed class WslToolkit(WslToolkitFamily family, IWslSetupInstance setupInstance) : IScriptableToolkit
{
    public string Name => "wsl";

    public string Description => setupInstance.DisplayName;

    public string? InstallationPath => setupInstance.InstallationPath;

    public IToolkitFamily Family => family;

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return ExecuteCommand(
            "sh \"`wslpath \"$0\"`\" \"$@\"",
            [NormalizePath(path), .. arguments],
            environment,
            options);
    }

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        string processPath = GetWslPath();

        var psi = new ProcessStartInfo
        {
            FileName = processPath,
            WorkingDirectory = NormalizePath(Directory.GetCurrentDirectory())
        };

        var processEnvironment = psi.Environment;
        EnvironmentServices.CombineEnvironmentWith(processEnvironment, environment);

        var processArguments = psi.ArgumentList;
        processArguments.Add("--exec");
        processArguments.Add("sh");
        processArguments.Add("-c");

        if (processEnvironment.ContainsKey("POSIXLY_CORRECT"))
        {
            var commandBuilder = new StringBuilder();
            commandBuilder.Append("export POSIXLY_CORRECT=;").Append(command);
            processArguments.Add(commandBuilder.ToString());
        }
        else
        {
            processArguments.Add(command);
        }

        processArguments.AddRange(arguments);

        return ToolkitKit.ExecuteProcess(psi);
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

    public string TranslateFilePath(string path)
    {
        string processPath = GetWslPath();

        var psi = new ProcessStartInfo
        {
            FileName = processPath,
            WorkingDirectory = NormalizePath(Directory.GetCurrentDirectory()),
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = true
        };

        var processArguments = psi.ArgumentList;
        processArguments.Add("--exec");
        processArguments.Add("wslpath");
        processArguments.Add(NormalizePath(path));

        var output = new StringBuilder();

        using var process =
            Process.Start(psi) ??
            throw new ProgramException(DiagnosticMessages.CannotStartProcess(psi.FileName));

        process.OutputDataReceived += Process_OutputDataReceived;

        process.BeginOutputReadLine();

        void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is var data and not (null or []))
            {
                if (output.Length != 0)
                    output.AppendLine();
                output.Append(data);
            }
        }

        process.WaitForExit();

        if (process.ExitCode != 0)
            return path;

        return output.ToString();
    }

    public ToolkitIsolation Isolation =>
        setupInstance.Version.Major switch
        {
            >= 2 => ToolkitIsolation.VirtualMachine,
            _ => ToolkitIsolation.Container
        };
}
