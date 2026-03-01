// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Provides GNU toolkit runtime functionality.
/// </summary>
abstract class ToolkitRuntime
{
    public virtual string ConvertPathToGuestFormat(string path, ToolkitPathConversionOptions options) => ConvertPath(path, options);

    public virtual string ConvertPathToHostFormat(string path, ToolkitPathConversionOptions options) => ConvertPath(path, options);

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
