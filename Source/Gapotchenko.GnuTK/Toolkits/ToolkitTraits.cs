// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Defines the traits of a GNU toolkit.
/// </summary>
[Flags]
public enum ToolkitTraits
{
    /// <summary>
    /// No traits.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indicates that a toolkit is built into GNU-TK.
    /// </summary>
    BuiltIn = 1 << 0
}
