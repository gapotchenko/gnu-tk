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
    public const string Command = "<command>";
    public const string Arguments = "<argument>";

    public const string ExecuteFile = "--file";
    public const string File = "<file>";

    public const string Toolkit = "--toolkit";
    public const string Strict = "--strict";

    public const string List = "list";
    public const string Check = "check";

    public static class Shorthands
    {
        public const string ExecuteCommand = "-c";
        public const string ExecuteCommandLine = "-l";
        public const string ExecuteFile = "-f";
    }
}
