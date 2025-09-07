// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits.Cygwin;

/// <summary>
/// Provides operations to mitigate quirks and inconsistencies in Cygwin.
/// </summary>
static class CygwinIdiosyncrasies
{
    /// <summary>
    /// Adjusts a command-line argument before passing it to a Cygwin-based
    /// toolkit, correcting distortions introduced by Cygwin.
    /// </summary>
    /// <param name="value">The original argument value.</param>
    /// <returns>The corrected argument value</returns>
    /// <remarks>
    /// <para>
    /// Cygwin and related toolchains alter the handling of the <c>\</c>
    /// character in command-line arguments. While this usually has no effect,
    /// the loss of semantic precision can break certain scenarios.
    /// </para>
    /// <para>
    /// Reasons of that behavior are not well understood and remain a mystery.
    /// </para>
    /// </remarks>
    public static string AdjustArgument(string value)
    {
        return value
            // "Undo" the distortions made by Cygwin.
            .Replace(@"\", @"\\", StringComparison.Ordinal);
    }
}
