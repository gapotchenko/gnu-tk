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
    /// <returns>The exit code.</returns>
    int ExecuteCommand(string command, IReadOnlyList<string> arguments);

    /// <summary>
    /// Executes the specified file.
    /// </summary>
    /// <param name="command">The path of a file to execute.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The exit code.</returns>
    int ExecuteFile(string path, IReadOnlyList<string> arguments);
}
