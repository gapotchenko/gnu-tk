// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Defines the traits of a GNU toolkit family.
/// </summary>
[Flags]
enum ToolkitFamilyTraits
{
    /// <summary>
    /// No traits.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indicates that a toolkit can be deployed by installing it on a computer system.
    /// </summary>
    Installable = 1 << 0,

    /// <summary>
    /// Indicates that a toolkit supports portable deployment.
    /// </summary>
    Portable = 1 << 1,

    /// <summary>
    /// The mask of traits related to deployment.
    /// </summary>
    DeployableMask = Installable | Portable
}
