// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// The base class for implementing a GNU toolkit runtime functionality.
/// </summary>
abstract class ToolkitRuntime
{
    /// <summary>
    /// Converts the specified file path to the toolkit's format.
    /// </summary>
    /// <param name="path">The file path to convert in the host system format.</param>
    /// <param name="options">The options.</param>
    /// <returns>A converted file path in the toolkit's format.</returns>
    public abstract string ConvertPathToGuestFormat(string path, ToolkitPathConversionOptions options);

    /// <summary>
    /// Converts the specified file path to the host system format.
    /// </summary>
    /// <param name="path">The file path to convert in the toolkit's format.</param>
    /// <param name="options">The options.</param>
    /// <returns>A converted file path in the host system format.</returns>
    public abstract string ConvertPathToHostFormat(string path, ToolkitPathConversionOptions options);
}
