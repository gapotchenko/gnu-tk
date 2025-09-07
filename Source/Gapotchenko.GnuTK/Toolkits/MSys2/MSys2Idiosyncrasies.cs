// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.GnuTK.Toolkits.Cygwin;

namespace Gapotchenko.GnuTK.Toolkits.MSys2;

/// <summary>
/// Provides operations to mitigate quirks and inconsistencies in MSYS2.
/// </summary>
static class MSys2Idiosyncrasies
{
    /// <inheritdoc cref="CygwinIdiosyncrasies.AdjustArgument(string)"/>
    public static string AdjustArgument(string value) => CygwinIdiosyncrasies.AdjustArgument(value);
}
