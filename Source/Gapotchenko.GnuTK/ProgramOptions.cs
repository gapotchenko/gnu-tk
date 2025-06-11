// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK;

/// <summary>
/// Defines the names of options used by the program.
/// </summary>
static class ProgramOptions
{
    public const string Help = "--help";
    public const string Quiet = "--quiet";
    public const string Version = "--version";

    public const string ExecuteCommand = "--command";
    public const string ExecuteCommandLine = "--command-line";
    public const string Commands = "<command>";
    public const string Arguments = "<argument>";

    public const string ExecuteFile = "--file";
    public const string File = "<file>";

    public const string Toolkit = "--toolkit";

    public const string List = "list";
    public const string Check = "check";
}
