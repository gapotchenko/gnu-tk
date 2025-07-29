// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO;

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Provides operations for toolkit environment manipulation.
/// </summary>
static class ToolkitEnvironment
{
    /// <summary>
    /// The name of <c>POSIXLY_CORRECT</c> environment variable.
    /// </summary>
    public const string PosixlyCorrect = "POSIXLY_CORRECT";

    public static Dictionary<string, string?> Create() => new(VariableNameComparer);

    [return: NotNullIfNotNull(nameof(a))]
    [return: NotNullIfNotNull(nameof(b))]
    public static IReadOnlyDictionary<string, string?>? Combine(
        IReadOnlyDictionary<string, string?>? a,
        IReadOnlyDictionary<string, string?>? b)
    {
        if (a is null)
            return b;
        if (b is null)
            return a;

        var environment = Create();
        foreach (string name in a.Keys.Union(b.Keys, VariableNameComparer))
            environment[name] = CombineValues(a, b, name);

        return environment;
    }

    public static void CombineWith(
        IDictionary<string, string?> environment,
        IReadOnlyDictionary<string, string?>? other)
    {
        if (other is null)
            return;

        var env = environment.AsReadOnly();
        foreach (string name in environment.Keys.ToList().Union(other.Keys, VariableNameComparer))
            environment[name] = CombineValues(env, other, name);
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

    public static void PrependPaths(
        IDictionary<string, string?> environment,
        params IEnumerable<string?> paths)
    {
        environment["PATH"] = JoinPaths(
            paths.Where(x => !string.IsNullOrEmpty(x))
            .Concat(SplitPath(environment.GetValueOrDefault("PATH") ?? string.Empty))
            .Distinct(FileSystem.PathComparer)!);
    }

    public static void RemovePaths(
        IDictionary<string, string?> environment,
        params IEnumerable<string?> paths)
    {
        if (environment.TryGetValue("PATH", out string? path) && !string.IsNullOrEmpty(path))
        {
            environment["PATH"] = JoinPaths(
                SplitPath(path)
                .Except(paths, FileSystem.PathComparer)!);
        }
    }

    static IEnumerable<string> SplitPath(string path) =>
        path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

    static string JoinPaths(params IEnumerable<string> paths) =>
        string.Join(Path.PathSeparator, paths);

    public static StringComparer VariableNameComparer { get; } =
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;
}
