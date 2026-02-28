// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Defines file path conversion options of a GNU toolkit.
/// </summary>
[Flags]
enum ToolkitPathConversionOptions
{
    /// <summary>
    /// No options.
    /// </summary>
    None = 0,

    /// <summary>
    /// Produce absolute path.
    /// </summary>
    Absolute = 1 << 0
}
