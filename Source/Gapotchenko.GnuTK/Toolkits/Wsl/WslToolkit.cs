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
        return ExecuteCommandCore(
            "sh \"`wslpath \"$0\"`\" \"$@\"",
            [NormalizePath(path), .. arguments],
            environment,
            null);
    }

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return ExecuteCommandCore(command, arguments, environment, ["-e"]);
    }

    int ExecuteCommandCore(
        string command,
        IReadOnlyList<string> commandArguments,
        IReadOnlyDictionary<string, string?>? environment,
        IEnumerable<string>? extraShellArguments)
    {
        string wslPath = GetWslPath();

        var psi = new ProcessStartInfo
        {
            FileName = wslPath,
            WorkingDirectory = NormalizePath(Directory.GetCurrentDirectory())
        };

        var processEnvironment = psi.Environment;
        EnvironmentServices.CombineEnvironmentWith(processEnvironment, environment);

        var wslArguments = psi.ArgumentList;
        wslArguments.Add("--exec");
        wslArguments.Add("sh");
        wslArguments.Add("-l");
        if (extraShellArguments != null)
            wslArguments.AddRange(extraShellArguments);
        wslArguments.Add("-c");

        var commandBuilder = new StringBuilder();

        string environmentScript = BuildEnvironmentScript(TranslateEnvironment(processEnvironment.AsReadOnly()));
        if (environmentScript is not [])
            commandBuilder.Append(environmentScript).Append(';');

        commandBuilder.Append(command);
        wslArguments.Add(commandBuilder.ToString());

        wslArguments.AddRange(commandArguments);

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

    static string BuildEnvironmentScript(IReadOnlyDictionary<string, string?> environment)
    {
        var builder = new StringBuilder();

        foreach (var (name, value) in environment)
        {
            if (builder.Length != 0)
                builder.Append(';');
            if (value is null)
                builder.Append("unset ").Append(name);
            else
                builder.Append("export ").Append(name).Append('=').Append(ToolkitKit.EscapeVariableValue(value));
        }

        return builder.ToString();
    }

    static IReadOnlyDictionary<string, string?> TranslateEnvironment(IReadOnlyDictionary<string, string?> environment)
    {
        var newEnvironment = new Dictionary<string, string?>(StringComparer.Ordinal);

        Translate("POSIXLY_CORRECT");
        Translate("GNU_TK");
        Translate("GNU_TK_TOOLKIT");

        if (!newEnvironment.ContainsKey("POSIXLY_CORRECT"))
            newEnvironment.Add("POSIXLY_CORRECT", null);

        return newEnvironment;

        void Translate(string name)
        {
            if (environment.TryGetValue(name, out string? value))
                newEnvironment[name] = value;
        }
    }

    public string TranslateFilePath(string path)
    {
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

        int exitCode = ToolkitKit.ExecuteProcess(psi, output);
        if (exitCode != 0)
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
