// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Represents a GNU toolkit that can execute scripts.
/// </summary>
interface IScriptableToolkit : IToolkit
{
    /// <summary>
    /// Gets the isolation level.
    /// </summary>
    ToolkitIsolation Isolation => ToolkitIsolation.None;

    /// <summary>
    /// Executes the specified shell command.
    /// </summary>
    /// <param name="command">The shell command to execute.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>The exit code.</returns>
    int ExecuteShellCommand(
        string command,
        IReadOnlyList<string> arguments,
        IReadOnlyDictionary<string, string?> environment,
        ToolkitExecutionOptions options);

    /// <summary>
    /// Executes the specified shell file.
    /// </summary>
    /// <param name="path">The path of a shell file to execute.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>The exit code.</returns>
    int ExecuteShellFile(
        string path,
        IReadOnlyList<string> arguments,
        IReadOnlyDictionary<string, string?> environment,
        ToolkitExecutionOptions options);

    /// <summary>
    /// Executes the specified file in the toolkit's environment.
    /// </summary>
    /// <param name="path">The path of a file to execute.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>The exit code.</returns>
    int ExecuteFile(
        string path,
        IReadOnlyList<string> arguments,
        IReadOnlyDictionary<string, string?> environment,
        ToolkitExecutionOptions options);

    /// <summary>
    /// Converts the specified file path to the toolkit's format.
    /// </summary>
    /// <param name="path">The file path to convert in the host system format.</param>
    /// <returns>A converted file path in the toolkit's format.</returns>
    string ConvertFilePathToGuestFormat(string path);

    /// <summary>
    /// Converts the specified file path to the host system format.
    /// </summary>
    /// <param name="path">The file path to convert in the toolkit's format.</param>
    /// <returns>A converted file path in the host system format.</returns>
    string ConvertFilePathToHostFormat(string path) => path;
}
