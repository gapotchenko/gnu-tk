// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Represents a GNU toolkit.
/// </summary>
interface IToolkit
{
    /// <summary>
    /// Gets the toolkit name.
    /// </summary>
    /// <remarks>
    /// For example: "msys2-ucrt64".
    /// </remarks>
    string Name { get; }

    /// <summary>
    /// Gets the toolkit description.
    /// </summary>
    /// <remarks>
    /// For example: "MSYS2 2025-02-21".
    /// </remarks>
    string Description { get; }

    /// <summary>
    /// Gets the toolkit installation path.
    /// </summary>
    /// <remarks>
    /// For example: "C:\msys64\ucrt64".
    /// </remarks>
    string? InstallationPath { get; }

    /// <summary>
    /// Gets the toolkit family.
    /// </summary>
    IToolkitFamily Family { get; }

    /// <summary>
    /// Executes the specified command.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <returns>The exit code.</returns>
    int ExecuteCommand(string command, IEnumerable<string> arguments);
}
