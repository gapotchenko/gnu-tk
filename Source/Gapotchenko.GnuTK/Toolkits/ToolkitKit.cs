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

        var environment = CreateEnvironment();
        foreach (string key in a.Keys.Concat(b.Keys).Distinct(FileSystem.PathComparer))
            environment[key] = CombineEnvironmentValues(a, b, key);

        return environment;
    }

    public static void CombineEnvironmentWith(
        IDictionary<string, string?> environment,
        IReadOnlyDictionary<string, string?>? other)
    {
        if (other is null)
            return;

        foreach (string key in environment.Keys.ToList().Concat(other.Keys).Distinct(FileSystem.PathComparer))
            environment[key] = CombineEnvironmentValues(environment, other, key);
    }

    static string? CombineEnvironmentValues(
        IReadOnlyDictionary<string, string?> a,
        IReadOnlyDictionary<string, string?> b,
        string key) =>
        (a.TryGetValue(key, out string? valueA), b.TryGetValue(key, out string? valueB)) switch
        {
            (true, true) => CombineEnvironmentValues(valueA, valueB, key),
            (true, false) => valueA,
            (false, true) => valueB,
            _ => throw new InvalidOperationException()
        };

    static string? CombineEnvironmentValues(
        IDictionary<string, string?> a,
        IReadOnlyDictionary<string, string?> b,
        string key) =>
        (a.TryGetValue(key, out string? valueA), b.TryGetValue(key, out string? valueB)) switch
        {
            (true, true) => CombineEnvironmentValues(valueA, valueB, key),
            (true, false) => valueA,
            (false, true) => valueB,
            _ => throw new InvalidOperationException()
        };

    static string? CombineEnvironmentValues(string? a, string? b, string key)
    {
        return
            key switch
            {
                "PATH" => CombineDelimitedValues(a, b, Path.PathSeparator, FileSystem.PathComparer),
                "CFLAGS" or "CPPFLAGS" or "LDFLAGS" => ConcatValues(a, b, ' '),
                _ => b
            };

        static string? CombineDelimitedValues(string? a, string? b, char delimiter, StringComparer comparer)
        {
            if (a is null)
                return b;
            if (b is null)
                return null;

            return string.Join(
                delimiter,
                b.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                    .Concat(a.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))
                    .Distinct(comparer));
        }

        static string? ConcatValues(string? a, string? b, char separator)
        {
            if (a is null)
                return b;
            if (b is null)
                return null;

            return b + separator + a;
        }
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
