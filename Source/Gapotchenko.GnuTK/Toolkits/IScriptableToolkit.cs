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
    ToolkitIsolation Isolation { get; }

    /// <summary>
    /// Executes the specified shell command.
    /// </summary>
    /// <param name="command">The shell command to execute.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>The exit code.</returns>
    int ExecuteCommand(
        string command,
        IReadOnlyList<string> arguments,
        IReadOnlyDictionary<string, string?>? environment,
        ToolkitExecutionOptions options);

    /// <summary>
    /// Executes the specified shell file.
    /// </summary>
    /// <param name="path">The path of a shell file to execute.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>The exit code.</returns>
    int ExecuteFile(
        string path,
        IReadOnlyList<string> arguments,
        IReadOnlyDictionary<string, string?>? environment,
        ToolkitExecutionOptions options);

    /// <summary>
    /// Translates the specified file path to the toolkit's format.
    /// </summary>
    /// <param name="path">The file path to translate in the host format.</param>
    /// <returns>A translated file path in the toolkit's format.</returns>
    string TranslateFilePath(string path);
}
