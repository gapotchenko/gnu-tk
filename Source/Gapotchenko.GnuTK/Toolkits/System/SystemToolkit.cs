// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.GnuTK.Diagnostics;
using Gapotchenko.GnuTK.Helpers;

namespace Gapotchenko.GnuTK.Toolkits.System;

sealed class SystemToolkit(SystemToolkitFamily family) : IScriptableToolkit
{
    public string Name => "system";

    public string Description =>
       family.Traits.HasFlag(ToolkitFamilyTraits.Alike)
            ? "Local GNU-like system."
            : "Local GNU system.";

    public string? InstallationPath => "/";

    public ToolkitTraits Traits => ToolkitTraits.None;

    public IToolkitFamily Family => family;

    public int ExecuteShellCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        return ExecuteShell(
            ["-e", "-c", command, .. arguments],
            environment,
            options);
    }

    public int ExecuteShellFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        return ExecuteShell(
            [path, .. arguments],
            environment,
            options);
    }

    int ExecuteShell(IEnumerable<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        return ExecuteFile(
            GetEnvPath(),
            ["sh", .. arguments],
            environment,
            options);
    }

    static string GetEnvPath()
    {
        string envPath = "/usr/bin/env";
        if (!File.Exists(envPath))
            throw new ProgramException(DiagnosticMessages.ModuleNotFound(envPath));
        return envPath;
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?> environment, ToolkitExecutionOptions options)
    {
        var psi = new ProcessStartInfo
        {
            FileName = path
        };

        ToolkitEnvironment.CombineWith(psi.Environment, environment);

        psi.ArgumentList.AddRange(arguments);

        return ProcessHelper.Execute(psi);
    }

    public string TranslateFilePath(string path) => path;
}
