// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Buffers;

namespace Gapotchenko.GnuTK.Helpers;

static class ShellHelper
{
    /// <summary>
    /// Escapes the specified string.
    /// This instructs the command shell to interpret the string characters literally rather than as metacharacters.
    /// </summary>
    /// <param name="value">The string to escape.</param>
    /// <returns>The escaped string.</returns>
    [return: NotNullIfNotNull(nameof(value))]
    public static string? Escape(string? value) =>
        value switch
        {
            null => null,
            [] => "''",
            var s when s.ContainsAny(m_Metacharacters) => "'" + s.Replace("'", "'\\''", StringComparison.Ordinal) + "'",
            _ => value
        };

    static readonly SearchValues<char> m_Metacharacters = SearchValues.Create(
    [
        // Command separators and control operators
        ';',  // Separates commands (e.g., cmd1; cmd2)
        '&',  // Background execution (e.g., cmd &) or logical AND (&&)
        '|',  // Pipe output or logical OR (||)

        // Redirection operators
        '<',  // Input redirection
        '>',  // Output redirection

        // Variable and command substitution
        '$',  // Variable substitution (e.g., $HOME)
        '`',  // Command substitution (e.g., `whoami`)
    
        // Quoting and escaping
        '\\', // Escape character
        '\'', // Single quote
        '"',  // Double quote

        // Subshells and grouping
        '(',  // Start of subshell or grouping
        ')',  // End of subshell or grouping
        '{',  // Start of command/group block or expansion
        '}',  // End of command/group block or expansion

        // Globbing (wildcard patterns)
        '*',  // Matches any number of characters
        '?',  // Matches any single character
        '[',  // Start of character class
        ']',  // End of character class

        // Miscellaneous
        '~',  // Home directory shortcut (e.g., ~user)
        '!',  // History expansion (in bash)

        // Code syntax
        '#',  // Comment (everything after is ignored)
        ' ',  // Separates expressions (e.g., arg1 arg2)
        '='   // Assignment operator (context-sensitive), used in variable assignments (e.g., NAME=value)
    ]);
}
