// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO;

namespace Gapotchenko.GnuTK;

/// <summary>
/// Provides operations for process environment manipulation.
/// </summary>
static class EnvironmentServices
{
    public static Dictionary<string, string?> CreateEnvironment() => new(VariableNameComparer);

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
        foreach (string name in a.Keys.Union(b.Keys, VariableNameComparer))
            environment[name] = CombineValues(a, b, name);

        return environment;
    }

    public static void CombineEnvironmentWith(
        IDictionary<string, string?> environment,
        IReadOnlyDictionary<string, string?>? other)
    {
        if (other is null)
            return;

        foreach (string name in environment.Keys.ToList().Union(other.Keys, VariableNameComparer))
            environment[name] = CombineValues(environment, other, name);
    }

    static string? CombineValues(
        IReadOnlyDictionary<string, string?> a,
        IReadOnlyDictionary<string, string?> b,
        string name) =>
        (a.TryGetValue(name, out string? valueA), b.TryGetValue(name, out string? valueB)) switch
        {
            (true, true) => CombineValues(valueA, valueB, name),
            (true, false) => valueA,
            (false, true) => valueB,
            _ => throw new InvalidOperationException()
        };

    static string? CombineValues(
        IDictionary<string, string?> a,
        IReadOnlyDictionary<string, string?> b,
        string name) =>
        (a.TryGetValue(name, out string? valueA), b.TryGetValue(name, out string? valueB)) switch
        {
            (true, true) => CombineValues(valueA, valueB, name),
            (true, false) => valueA,
            (false, true) => valueB,
            _ => throw new InvalidOperationException()
        };

    static string? CombineValues(string? a, string? b, string name)
    {
        return
            name switch
            {
                "PATH" => CombineSeparatedValues(a, b, Path.PathSeparator, FileSystem.PathComparer),
                "CFLAGS" or "CPPFLAGS" or "LDFLAGS" => ConcatValues(a, b, ' '),
                _ => b
            };

        static string? CombineSeparatedValues(string? a, string? b, char separator, StringComparer comparer)
        {
            if (a is null)
                return b;
            if (b is null)
                return null;

            return string.Join(
                separator,
                b.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                    .Concat(a.Split(separator, StringSplitOptions.RemoveEmptyEntries))
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

    public static IEnumerable<string> SplitPath(string value) =>
        value.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

    public static string JoinPath(params IEnumerable<string> paths) =>
        string.Join(Path.PathSeparator, paths);

    public static StringComparer VariableNameComparer { get; } =
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;
}
