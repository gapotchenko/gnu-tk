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

    #region Deployment

    /// <summary>
    /// Indicates that a toolkit can be deployed by installing it on a computer system.
    /// </summary>
    Installable = 1 << 0,

    /// <summary>
    /// Indicates that a toolkit family supports portable deployment.
    /// </summary>
    Portable = 1 << 1,

    /// <summary>
    /// The mask of deployment traits.
    /// </summary>
    DeploymentMask = Installable | Portable,

    #endregion

    /// <summary>
    /// Indicates that a toolkit family provides an alike but not the exact GNU semantics.
    /// </summary>
    /// <remarks>
    /// The trait allows to differentiate between GNU and GNU-like toolkit families.
    /// </remarks>
    Alike = 1 << 2,

    /// <summary>
    /// Indicates that a toolkit requires file paths to be translated.
    /// </summary>
    FilePathTranslation = 1 << 3
}
