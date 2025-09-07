// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.GnuTK.Toolkits.Cygwin;

namespace Gapotchenko.GnuTK.Toolkits.Git;

/// <summary>
/// Provides operations to mitigate quirks and inconsistencies in a GNU toolkit
/// that comes with "Git for Windows".
/// </summary>
static class GitIdiosyncrasies
{
    /// <inheritdoc cref="CygwinIdiosyncrasies.AdjustArgument(string)"/>
    public static string AdjustArgument(string value) => CygwinIdiosyncrasies.AdjustArgument(value);
}
