// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.GnuTK.Diagnostics;

namespace Gapotchenko.GnuTK.Helpers;

static class ProcessHelper
{
    public static int Execute(ProcessStartInfo psi)
    {
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        using var process =
            Process.Start(psi) ??
            throw new InvalidOperationException(DiagnosticMessages.CannotStartProcess(psi.FileName));
        process.WaitForExit();
        return process.ExitCode;
    }

    public static int Execute(ProcessStartInfo psi, TextWriter output)
    {
        psi.CreateNoWindow = true;
        psi.RedirectStandardOutput = true;

        using var process =
            Process.Start(psi) ??
            throw new InvalidOperationException(DiagnosticMessages.CannotStartProcess(psi.FileName));

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
}
