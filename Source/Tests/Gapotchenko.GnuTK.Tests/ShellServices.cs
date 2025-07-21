// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;

namespace Gapotchenko.GnuTK.Tests;

static class ShellServices
{
    public static int ExecuteProcess(
        string filePath,
        IEnumerable<string> arguments,
        string? workingDirectory = null)
    {
        var psi = new ProcessStartInfo
        {
            FileName = filePath,
            CreateNoWindow = true
        };

        psi.ArgumentList.AddRange(arguments);

        if (workingDirectory != null)
            psi.WorkingDirectory = workingDirectory;

        using var process =
            Process.Start(psi) ??
            throw new InvalidOperationException(string.Format("Cannot start process '{0}'.", psi.FileName));
        process.WaitForExit();
        return process.ExitCode;
    }
}
