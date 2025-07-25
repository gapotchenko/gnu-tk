// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Helpers;

static class ShellHelper
{
    [return: NotNullIfNotNull(nameof(value))]
    public static string? EscapeVariableValue(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        // TODO: needs more thorough implementation.

        return value.Replace(@"\", @"\\");
    }
}
