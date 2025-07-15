// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.GnuTK.Diagnostics;

namespace Gapotchenko.GnuTK.Toolkits.System;

sealed class SystemToolkit(SystemToolkitFamily family) : IScriptableToolkit
{
    public string Name => "system";

    public string Description =>
       family.Traits.HasFlag(ToolkitFamilyTraits.Alike)
            ? "Local GNU-like system."
            : "Local GNU system.";

    public string? InstallationPath => "/";

    public IToolkitFamily Family => family;

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return ExecuteShell(["-c", command, .. arguments], environment, options);
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return ExecuteShell([path, .. arguments], environment, options);
    }

    static int ExecuteShell(IEnumerable<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        string envPath = GetEnvPath();

        var psi = new ProcessStartInfo
        {
            FileName = envPath
        };

        if (environment != null)
            EnvironmentServices.CombineEnvironmentWith(psi.Environment, environment);

        var args = psi.ArgumentList;
        args.Add("sh");
        args.AddRange(arguments);

        return ToolkitKit.ExecuteProcess(psi);
    }

    static string GetEnvPath()
    {
        string envPath = "/usr/bin/env";
        if (!File.Exists(envPath))
            throw new ProgramException(DiagnosticMessages.ModuleNotFound(envPath));
        return envPath;
    }

    public string TranslateFilePath(string path) => path;
}
