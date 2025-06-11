// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX;
using Gapotchenko.GnuTK.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.GnuTK;

static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        try
        {
            CliServices.InitializeConsole();
            return Run(args);
        }
        catch (ProgramExitException e)
        {
            return e.ExitCode;
        }
        catch (Exception e)
        {
            UIShell.ShowError(e);
            return 1;
        }
    }

    static int Run(IReadOnlyList<string> args)
    {
        string usage =
            """
            Usage:
              gnu-tk [-t <name>] -c [--] <command> [<argument>...]
              gnu-tk [-t <name>] -l [--] <argument>...
              gnu-tk [-t <name>] -f [--] <file> [<argument>...]
              gnu-tk (list | check [-t <name>]) [-q]
              gnu-tk (-h | --help) [-q]
              gnu-tk --version

            Options:
              -h --help            Show this help.
              --version            Show version.            
              -c --command         Execute a command using a GNU toolkit.
              -l --command-line    Execute a command line using a GNU toolkit.
              -f --file            Execute a file using a GNU toolkit.
              -t --toolkit=<name>  Use the specified GNU toolkit [default: auto].
              -q --quiet           Do not print any auxiliary messages.
            
            Commands:
              list                 List available GNU toolkits.
              check                Check a GNU toolkit.
            """;

        var arguments =
            CliServices.TryParseArguments(CanonicalizeArgs(args), usage) ??
            throw new DiagnosticException("Invalid program arguments.", DiagnosticCode.InvalidProgramArguments);

        if (UIShell.Run(arguments, usage, out int exitCode))
            return exitCode;
        else
            return RunCore(arguments);
    }

    #region Canonicalization of program arguments

    /// <summary>
    /// Inteprets naturally occuring program arguments and
    /// rewrites them in canononical form suitable for the formal parsing.
    /// </summary>
    static IReadOnlyList<string> CanonicalizeArgs(IReadOnlyList<string> args)
    {
        int n = args.Count;
        if (n == 0)
            return [ProgramOptions.Help];

        bool windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var newArgs = new List<string>(n + 1);

        var state = ArgsCanonicalizationState.Begin;
        for (int i = 0; i < n; ++i)
        {
            string arg = args[i];

            switch (state)
            {
                case ArgsCanonicalizationState.Begin:
                    switch (arg)
                    {
                        case "-?":
                        case "/?" when windows:
                            arg = ProgramOptions.Help;
                            state = ArgsCanonicalizationState.End;
                            break;

                        case ProgramOptions.ExecuteCommand or ProgramOptions.Shorthands.ExecuteCommand:
                        case ProgramOptions.ExecuteCommandLine or ProgramOptions.Shorthands.ExecuteCommandLine:
                        case ProgramOptions.ExecuteFile or ProgramOptions.Shorthands.ExecuteFile:
                            state = ArgsCanonicalizationState.StartPositional;
                            break;
                    }
                    break;

                case ArgsCanonicalizationState.StartPositional:
                    if (arg != "--")
                    {
                        // Ensures that this and the next arguments will not be parsed as named parameters.
                        newArgs.Add("--");
                    }
                    state = ArgsCanonicalizationState.End;
                    break;

                case ArgsCanonicalizationState.End:
                    break;
            }

            newArgs.Add(arg);
        }

        return newArgs;
    }

    enum ArgsCanonicalizationState
    {
        Begin,
        StartPositional,
        End
    }

    #endregion

    [MethodImpl(MethodImplOptions.NoInlining)] // avoid premature initialization of types
    static int RunCore(IReadOnlyDictionary<string, object> arguments)
    {
        // Initialize the engine.

        var engine = new Engine()
        {
            ToolkitName =
                NormalizeToolkitName((string)arguments[ProgramOptions.Toolkit])
                ?? NormalizeToolkitName(Environment.GetEnvironmentVariable("GNU_TK_TOOLKIT")),
            ToolkitPaths = GetToolkitPaths(),
            Quiet = (bool)arguments[ProgramOptions.Quiet]
        };

        static string? NormalizeToolkitName(string? name) =>
            string.IsNullOrEmpty(name) ||
            name.Equals("auto", StringComparison.OrdinalIgnoreCase)
                ? null
                : name;

        static IReadOnlyList<string> GetToolkitPaths() =>
            Environment.GetEnvironmentVariable("GNU_TK_TOOLKIT_PATH")
            ?.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? [];

        // Handle the program options.
        // Most frequently used options are handled first.

        if ((bool)arguments[ProgramOptions.ExecuteCommand])
        {
            string command = (string)arguments[ProgramOptions.Command];
            var commandArguments = (IReadOnlyList<string>)arguments[ProgramOptions.Arguments];
            return engine.ExecuteCommand(command, commandArguments);
        }

        if ((bool)arguments[ProgramOptions.ExecuteFile])
        {
            string filePath = (string)arguments[ProgramOptions.File];
            var fileArguments = (IReadOnlyList<string>)arguments[ProgramOptions.Arguments];
            return engine.ExecuteFile(filePath, fileArguments);
        }

        if ((bool)arguments[ProgramOptions.ExecuteCommandLine])
        {
            var commandLine = Environment.CommandLine.AsSpan();

            // A bit of magic.
            // In Windows OS, the native representation of a command line is a string.
            // Take a benefit from that fact by directly extracting a command to execute from the string.

            // Find the start of a specified command line.
            const string key1 = $" {ProgramOptions.Shorthands.ExecuteCommandLine} ";
            int j = commandLine.IndexOf(key1, StringComparison.Ordinal);
            if (j != -1)
            {
                j += key1.Length;
            }
            else
            {
                const string key2 = $" {ProgramOptions.ExecuteCommandLine} ";
                j = commandLine.IndexOf(key2, StringComparison.Ordinal);
                if (j != -1)
                    j += key2.Length;
            }
            if (j == -1)
                throw new ProductException("Cannot find a command line start position.");
            var command = commandLine[j..];

            // Remove an optional delimiter for positional arguments.
            const string positionalDelimeter = "-- ";
            if (command.StartsWith(positionalDelimeter, StringComparison.Ordinal))
                command = command[positionalDelimeter.Length..];

            return engine.ExecuteCommand(command.ToString(), []);
        }

        if ((bool)arguments[ProgramOptions.List])
        {
            engine.ListToolkits();
            return 0;
        }

        if ((bool)arguments[ProgramOptions.Check])
            return engine.CheckToolkit() ? 0 : 1;

        CliServices.DumpArguments(arguments);
        return 1;
    }
}
