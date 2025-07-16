// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Defines toolkit isolation levels.
/// </summary>
[Flags]
enum ToolkitIsolation
{
    /// <summary>
    /// No isolation.
    /// </summary>
    None,

    /// <summary>
    /// Toolkit works in a virtual machine.
    /// </summary>
    VirtualMachine,

    /// <summary>
    /// Toolkit works in a container.
    /// </summary>
    Container
}
