// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits;

[Flags]
enum ToolkitExecutionOptions
{
    /// <summary>
    /// No options.
    /// </summary>
    None,

    /// <summary>
    /// Use strict GNU semantics.
    /// </summary>
    Strict = 1 << 0
}
