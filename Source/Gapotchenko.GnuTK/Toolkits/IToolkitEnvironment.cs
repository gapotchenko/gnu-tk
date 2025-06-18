// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Represents a GNU toolkit that provides environment.
/// </summary>
interface IToolkitEnvironment : IToolkit
{
    /// <summary>
    /// Gets the toolkit environment.
    /// </summary>
    IReadOnlyDictionary<string, string?>? Environment { get; }
}
