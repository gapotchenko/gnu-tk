// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Provides native GNU toolkit runtime functionality.
/// </summary>
sealed class NativeToolkitRuntime : ToolkitRuntime
{
    public static NativeToolkitRuntime Instance { get; } = new();

    NativeToolkitRuntime()
    {
    }

    public override string ConvertPathToGuestFormat(string path, ToolkitPathConversionOptions options) => ConvertPath(path, options);

    public override string ConvertPathToHostFormat(string path, ToolkitPathConversionOptions options) => ConvertPath(path, options);

    static string ConvertPath(string path, ToolkitPathConversionOptions options)
    {
        // Edge cases
        if (path.Length == 0)
            return path;

        // Options
        if (options.HasFlag(ToolkitPathConversionOptions.Absolute))
            path = Path.GetFullPath(path);

        return path;
    }
}
