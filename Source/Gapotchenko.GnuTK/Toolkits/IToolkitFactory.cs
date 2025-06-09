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
interface IToolkitFactory
{
    /// <summary>
    /// Gets the toolkit display name.
    /// </summary>
    /// <remarks>
    /// For example, "MSYS2".
    /// </remarks>
    string Name { get; }
}
