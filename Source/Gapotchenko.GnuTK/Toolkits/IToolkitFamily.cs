// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Represents a GNU toolkit factory.
/// </summary>
interface IToolkitFamily
{
    /// <summary>
    /// Gets the toolkit family name.
    /// </summary>
    /// <remarks>
    /// For example, "MSYS2".
    /// </remarks>
    string Name { get; }

    /// <summary>
    /// Gets the toolkit family traits.
    /// </summary>
    ToolkitFamilyTraits Traits { get; }

    /// <summary>
    /// Enumerates toolkits installed on a computer system.
    /// </summary>
    /// <returns>A sequence of discovered toolkits.</returns>
    IEnumerable<IToolkit> EnumerateInstalledToolkits();

    /// <summary>
    /// Enumerates portable toolkits from a directory.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <returns>A sequence of discovered toolkits.</returns>
    IEnumerable<IToolkit> EnumerateToolkitsFromDirectory(string path);
}
