// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.GnuTK.Diagnostics;

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// The kit for implementing toolkits' support.
/// </summary>
static class ToolkitKit
{
    public static int ExecuteProcess(ProcessStartInfo psi)
    {
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        using var process =
            Process.Start(psi) ??
            throw new ProgramException(DiagnosticMessages.CannotStartProcess(psi.FileName));
        process.WaitForExit();
        return process.ExitCode;
    }
}
