// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.GnuTK.Toolkits;

static class ToolkitIsolationFormatter
{
    public static string GetString(ToolkitIsolation value)
    {
        return value switch
        {
            ToolkitIsolation.None => "none",
            ToolkitIsolation.VirtualMachine => "vm",
            ToolkitIsolation.Container => "container"
        };
    }

    public static ToolkitIsolation Parse(ReadOnlySpan<char> s)
    {
        return s switch
        {
            "none" => ToolkitIsolation.None,
            "vm" => ToolkitIsolation.VirtualMachine,
            "container" => ToolkitIsolation.Container,
            _ => throw new FormatException("Invalid toolkit isolation value.")
        };
    }
}
