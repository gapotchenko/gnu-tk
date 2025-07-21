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
        string? workingDirectory = null,
        IReadOnlyDictionary<string, string?>? environment = null)
    {
        var psi = new ProcessStartInfo
        {
            FileName = filePath,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        psi.ArgumentList.AddRange(arguments);

        if (workingDirectory != null)
            psi.WorkingDirectory = workingDirectory;

        if (environment != null)
        {
            var processEnvironment = psi.Environment;
            foreach (var (key, value) in environment)
                processEnvironment[key] = value;
        }

        using var process =
            Process.Start(psi) ??
            throw new InvalidOperationException(string.Format("Cannot start process '{0}'.", psi.FileName));

        process.OutputDataReceived += (sender, e) => Process_DataReceived(Console.Out, e);
        process.ErrorDataReceived += (sender, e) => Process_DataReceived(Console.Error, e);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();
        return process.ExitCode;
    }

    static void Process_DataReceived(TextWriter textWriter, DataReceivedEventArgs e)
    {
        if (e.Data is { } data)
            textWriter.WriteLine(data);
    }
}
