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
    /// Gets the name.
    /// </summary>
    /// <remarks>
    /// For example, "MSYS2".
    /// </remarks>
    string Name { get; }

    /// <summary>
    /// Gets the name aliases.
    /// </summary>
    IReadOnlyList<string> Aliases { get; }

    /// <summary>
    /// Gets the traits.
    /// </summary>
    ToolkitFamilyTraits Traits { get; }

    /// <summary>
    /// Enumerates toolkits installed on a computer system.
    /// </summary>
    /// <returns>A sequence of discovered toolkits.</returns>
    IEnumerable<IToolkit> EnumerateInstalledToolkits();

    /// <summary>
    /// Enumerates portable toolkits in a specified directory.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <param name="traits">The minimal set of traits a discovered toolkit should declare.</param>
    /// <returns>A sequence of discovered toolkits.</returns>
    IEnumerable<IToolkit> EnumerateToolkitsInDirectory(string path, ToolkitTraits traits);
}
