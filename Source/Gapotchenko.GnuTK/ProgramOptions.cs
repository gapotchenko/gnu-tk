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

    public const string ExecuteShellCommand = "--command";
    public const string ExecuteShellCommandLine = "--command-line";
    public const string Command = "<command>";
    public const string Arguments = "<argument>";

    public const string ExecuteShellFile = "--file";
    public const string ExecuteFile = "--execute";
    public const string File = "<file>";

    public const string Toolkit = "--toolkit";
    public const string Strict = "--strict";
    public const string Integrated = "--integrated";
    public const string Posix = "--posix";

    public const string List = "list";
    public const string Check = "check";

    public static class Shorthands
    {
        public const string Quiet = "-q";

        public const string ExecuteShellCommand = "-c";
        public const string ExecuteShellCommandLine = "-l";

        public const string ExecuteShellFile = "-f";
        public const string ExecuteFile = "-x";

        public const string Toolkit = "-t";
        public const string Strict = "-s";
        public const string Integrated = "-i";
        public const string Posix = "-p";
    }
}
