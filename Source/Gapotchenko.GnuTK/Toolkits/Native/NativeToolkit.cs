// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.GnuTK.Diagnostics;

namespace Gapotchenko.GnuTK.Toolkits.Native;

sealed class NativeToolkit(NativeToolkitFamily family) : IExecutingToolkit
{
    public string Name => "native";

    public string Description =>
       family.Traits.HasFlag(ToolkitFamilyTraits.Alike)
            ? "A native GNU-like system."
            : "A native GNU system.";

    public string? InstallationPath => "/";

    public IToolkitFamily Family => family;

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments)
    {
        string envPath = GetEnvPath();

        var psi = new ProcessStartInfo
        {
            FileName = envPath
        };

        var args = psi.ArgumentList;
        args.Add("sh");
        args.Add("-c");
        args.Add(command);
        args.AddRange(arguments);

        return ToolkitKit.ExecuteProcess(psi);
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments)
    {
        string envPath = GetEnvPath();

        var psi = new ProcessStartInfo
        {
            FileName = envPath
        };

        var args = psi.ArgumentList;
        args.Add("sh");
        args.Add(path);
        args.AddRange(arguments);

        return ToolkitKit.ExecuteProcess(psi);
    }

    static string GetEnvPath()
    {
        string envPath = "/usr/bin/env";
        if (!File.Exists(envPath))
            throw new ProductException(DiagnosticMessages.ModuleNotFound(envPath));
        return envPath;
    }
}
