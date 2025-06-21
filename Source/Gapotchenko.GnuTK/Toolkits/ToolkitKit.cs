// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO;
using Gapotchenko.GnuTK.Diagnostics;

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// The kit for implementing toolkits' support.
/// </summary>
static class ToolkitKit
{
    public static Dictionary<string, string?> CreateEnvironment() => new(FileSystem.PathComparer);

    [return: NotNullIfNotNull(nameof(a))]
    [return: NotNullIfNotNull(nameof(b))]
    public static IReadOnlyDictionary<string, string?>? CombineEnvironments(
        IReadOnlyDictionary<string, string?>? a,
        IReadOnlyDictionary<string, string?>? b)
    {
        if (a is null)
            return b;
        if (b is null)
            return a;

        // TODO

        throw new NotImplementedException();
    }

    public static void CombineEnvironmentWith(
        IDictionary<string, string?> environment,
        IReadOnlyDictionary<string, string?>? other)
    {
        if (other is null)
            return;

        // TODO

        throw new NotImplementedException();
    }

    public static int ExecuteProcess(ProcessStartInfo psi)
    {
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        using var process =
            Process.Start(psi) ??
            throw new ProductException(DiagnosticMessages.CannotStartProcess(psi.FileName));
        process.WaitForExit();
        return process.ExitCode;
    }
}
