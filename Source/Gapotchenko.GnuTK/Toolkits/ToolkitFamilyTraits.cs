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
    /// Indicates that a toolkit family supports portable deployment.
    /// </summary>
    Portable = 1 << 1,

    /// <summary>
    /// Indicates that a toolkit family provides a strict GNU semantics.
    /// </summary>
    /// <remarks>
    /// The trait allows to differentiate between GNU and GNU-like toolkit families.
    /// </remarks>
    Strict = 1 << 2,

    /// <summary>
    /// The mask of deployment traits.
    /// </summary>
    DeploymentMask = Installable | Portable
}
