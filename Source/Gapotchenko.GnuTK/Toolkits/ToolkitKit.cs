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
        ConfigureProcess(psi);
        using var process =
            Process.Start(psi) ??
            throw new ProgramException(DiagnosticMessages.CannotStartProcess(psi.FileName));
        process.WaitForExit();
        return process.ExitCode;
    }

    public static int ExecuteProcess(ProcessStartInfo psi, TextWriter output)
    {
        ConfigureProcess(psi);
        psi.RedirectStandardOutput = true;

        using var process =
            Process.Start(psi) ??
            throw new ProgramException(DiagnosticMessages.CannotStartProcess(psi.FileName));

        bool hasOutput = false;

        void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is { } data)
            {
                if (hasOutput)
                    output.WriteLine();
                output.Write(data);
                hasOutput = true;
            }
        }

        process.OutputDataReceived += Process_OutputDataReceived;
        process.BeginOutputReadLine();

        process.WaitForExit();

        return process.ExitCode;
    }

    static void ConfigureProcess(ProcessStartInfo psi)
    {
        psi.WindowStyle = ProcessWindowStyle.Hidden;
    }
}
