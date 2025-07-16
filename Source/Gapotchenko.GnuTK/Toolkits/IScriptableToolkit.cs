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
    /// Executes the specified command.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="arguments">The command arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>The exit code.</returns>
    int ExecuteCommand(
        string command,
        IReadOnlyList<string> arguments,
        IReadOnlyDictionary<string, string?>? environment,
        ToolkitExecutionOptions options);

    /// <summary>
    /// Executes the specified file.
    /// </summary>
    /// <param name="command">The path of a file to execute.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="options">The options.</param>
    /// <returns>The exit code.</returns>
    int ExecuteFile(
        string path,
        IReadOnlyList<string> arguments,
        IReadOnlyDictionary<string, string?>? environment,
        ToolkitExecutionOptions options);

    /// <summary>
    /// Gets the isolation level.
    /// </summary>
    ToolkitIsolation Isolation { get; }

    /// <summary>
    /// Translates the specified file path to the toolkit's format.
    /// </summary>
    /// <param name="paths">The file path to translate.</param>
    /// <returns>A translated file path in the toolkit's format.</returns>
    string TranslateFilePath(string path);
}
