// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX;
using Gapotchenko.FX.Diagnostics;
using Gapotchenko.GnuTK.Diagnostics;
using Gapotchenko.GnuTK.Toolkits;
using Gapotchenko.GnuTK.UI;
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
        if (args is [])
        {
            var writer = Console.Error;

            using (UIStyles.Scope.Error(writer))
            {
                UIStyles.ErrorPrologue(writer, DiagnosticCode.InvalidProgramArguments);
                writer.WriteLine(DiagnosticMessages.InvalidProgramArguments);
            }

            writer.WriteLine();
            writer.WriteLine("Try 'gnu-tk --help' for more information.");

            return 1;
        }

        string usage =
            """
            Usage:
              gnu-tk [-t <name>] [-s] [-i] [-p] -c [--] <command> [<argument>...]
              gnu-tk [-t <name>] [-s] [-i] [-p] -l [--] <argument>...
              gnu-tk [-t <name>] [-s] [-i] [-p] (-f | -x) [--] <file> [<argument>...]
              gnu-tk (list | check [-t <name>]) [-s] [-i] [-q]
              gnu-tk (--help | --version) [-q]

            Options:
              --help               Show this help.
              --version            Show version.            
              -c --command         Execute a command using a GNU shell.
              -l --command-line    Execute a command line using a GNU shell.
              -f --file            Execute a script using a GNU shell.
              -x --execute         Execute a file in a GNU environment.
              -t --toolkit=<name>  Use the specified GNU toolkit [default: auto].
              -s --strict          Use a toolkit with strict GNU semantics.
              -i --integrated      Use a toolkit that operates within the host environment.
              -p --posix           Enables POSIX-compliant behavior.
              -q --quiet           Do not print any auxiliary messages.

            Commands:
              list                 List available GNU toolkits.
              check                Check a GNU toolkit.
            """;

        var parsedArguments =
            CliServices.TryParseArguments(CanonicalizeArgs(ExpandArgs(args)), usage) ??
            throw new DiagnosticException(DiagnosticMessages.InvalidProgramArguments, DiagnosticCode.InvalidProgramArguments);

        if (UIShell.Run(parsedArguments, usage, out int exitCode))
            return exitCode;
        else
            return RunCore(parsedArguments);
    }

    #region Program arguments expansion & canonicalization

    /// <summary>
    /// Expands naturally occurring program arguments and
    /// rewrites them in expanded form suitable for the formal parsing.
    /// </summary>
    static IReadOnlyList<string> ExpandArgs(IReadOnlyList<string> args)
    {
        return ExpandShebangArgs(args);

        /// Expands shebang arguments from <c>"-a -b" c</c> form to <c>-a -b c</c>.
        static IReadOnlyList<string> ExpandShebangArgs(IReadOnlyList<string> args)
        {
            if (args.Count != 2)
                return args;

            string arg0 = args[0];
            if (!(arg0.StartsWith('-') && arg0.Contains(' ')))
                return args;

            // Linearize shebang arguments.
            return [.. CommandLine.Split(arg0).Concat(args.Skip(1))];
        }
    }

    /// <summary>
    /// Interprets naturally occurring program arguments and
    /// rewrites them in canonical form suitable for the formal parsing.
    /// Performs additional arguments validation.
    /// </summary>
    static IReadOnlyList<string> CanonicalizeArgs(IReadOnlyList<string> args)
    {
        int n = args.Count;
        if (n == 0)
            return args;

        var newArgs = new List<string>(n + 1);

        var state = ArgsCanonicalizationState.Option;
        bool hasCommand = false;

        for (int i = 0; i < n; ++i)
        {
            string arg = args[i];

            switch (state)
            {
                case ArgsCanonicalizationState.Option:
                    switch (arg)
                    {
                        case ProgramOptions.ExecuteShellCommand or ProgramOptions.Shorthands.ExecuteShellCommand:
                        case ProgramOptions.ExecuteShellCommandLine or ProgramOptions.Shorthands.ExecuteShellCommandLine:
                        case ProgramOptions.ExecuteShellFile or ProgramOptions.Shorthands.ExecuteShellFile:
                        case ProgramOptions.ExecuteFile or ProgramOptions.Shorthands.ExecuteFile:
                            if (hasCommand)
                                return [];
                            state = ArgsCanonicalizationState.Positional;
                            break;

                        case ProgramOptions.Toolkit or ProgramOptions.Shorthands.Toolkit:
                            state = ArgsCanonicalizationState.Argument;
                            break;

                        case "-?" or "/?" when RuntimeInformation.IsOSPlatform(OSPlatform.Windows):
                            // A long-standing tradition on Windows.
                            arg = ProgramOptions.Help;
                            break;

                        case ProgramOptions.List:
                        case ProgramOptions.Check:
                            hasCommand = true;
                            break;

                        case ProgramOptions.Help:
                        case ProgramOptions.Quiet or ProgramOptions.Shorthands.Quiet:
                        case ProgramOptions.Version:
                        case ProgramOptions.Strict or ProgramOptions.Shorthands.Strict:
                        case ProgramOptions.Integrated or ProgramOptions.Shorthands.Integrated:
                        case ProgramOptions.Posix or ProgramOptions.Shorthands.Posix:
                        case "-sp": // strict + posix
                        case "-si": // strict + integrated
                        case "-sip": // strict + integrated + posix
                        case "-ip": // integrated + posix
                            break;

                        default:
                            // Invalid arguments.
                            return [];
                    }
                    break;

                case ArgsCanonicalizationState.Argument:
                    state = ArgsCanonicalizationState.Option;
                    break;

                case ArgsCanonicalizationState.Positional:
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
        Option,
        Argument,
        Positional,
        End
    }

    #endregion

    [MethodImpl(MethodImplOptions.NoInlining)] // avoid premature initialization of types
    static int RunCore(IReadOnlyDictionary<string, object> arguments)
    {
        // Initialize a GNU-TK engine.
        var engine = InitializeEngine(arguments);

        if (RunEngine(engine, arguments, out int exitCode))
            return exitCode;

        throw new InternalException("Unhandled command-line arguments.");
    }

    /// <summary>
    /// Creates and initializes a GNU-TK engine.
    /// </summary>
    static Engine InitializeEngine(IReadOnlyDictionary<string, object> arguments)
    {
        return new Engine()
        {
            ToolkitNames = GetToolkitNames(arguments),
            ToolkitPaths = GetToolkitPaths(),
            ToolkitIsolationLevels = GetToolkitIsolationLevels(arguments),
            Strict = (bool)arguments[ProgramOptions.Strict] || Environment.GetEnvironmentVariable("GNU_TK_STRICT") != null,
            Posix = (bool)arguments[ProgramOptions.Posix],
            Quiet = (bool)arguments[ProgramOptions.Quiet]
        };

        static IReadOnlyList<string>? GetToolkitNames(IReadOnlyDictionary<string, object> arguments)
        {
            return
                (TryParseNames((string)arguments[ProgramOptions.Toolkit]) ??
                TryParseNames(Environment.GetEnvironmentVariable("GNU_TK_TOOLKIT")))
                ?.Distinct(StringComparer.OrdinalIgnoreCase)
                ?.ToArray();

            static IReadOnlyList<string>? TryParseNames(string? names)
            {
                if (string.IsNullOrEmpty(names) ||
                    names.Equals("auto", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                else
                {
                    return names.Split(
                        [',', ';'],
                        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                }
            }
        }

        static IReadOnlyList<string> GetToolkitPaths() =>
            Environment.GetEnvironmentVariable("GNU_TK_TOOLKIT_PATH")
            ?.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ??
            [];

        static IReadOnlyList<ToolkitIsolation>? GetToolkitIsolationLevels(IReadOnlyDictionary<string, object> arguments)
        {
            if ((bool)arguments[ProgramOptions.Integrated])
                return [ToolkitIsolation.None];
            else
                return null;
        }
    }

    static bool RunEngine(Engine engine, IReadOnlyDictionary<string, object> arguments, out int exitCode)
    {
        // Most frequently used options are handled first.

        // '-c' command-line option (execute a shell command).
        if ((bool)arguments[ProgramOptions.ExecuteShellCommand])
        {
            string command = (string)arguments[ProgramOptions.Command];
            var commandArguments = (IReadOnlyList<string>)arguments[ProgramOptions.Arguments];
            exitCode = engine.ExecuteShellCommand(command, commandArguments);
            return true;
        }

        // '-l' command-line option (execute a shell command line).
        if ((bool)arguments[ProgramOptions.ExecuteShellCommandLine])
        {
            var commandLineArguments = (IReadOnlyList<string>)arguments[ProgramOptions.Arguments];
            exitCode = engine.ExecuteShellCommandLine(commandLineArguments);
            return true;
        }

        // '-f' command-line option (execute a shell script file).
        if ((bool)arguments[ProgramOptions.ExecuteShellFile])
        {
            string scriptPath = (string)arguments[ProgramOptions.File];
            var scriptArguments = (IReadOnlyList<string>)arguments[ProgramOptions.Arguments];
            exitCode = engine.ExecuteShellFile(scriptPath, scriptArguments);
            return true;
        }

        // '-x' command-line option (execute a file in a GNU environment).
        if ((bool)arguments[ProgramOptions.ExecuteFile])
        {
            string filePath = (string)arguments[ProgramOptions.File];
            var fileArguments = (IReadOnlyList<string>)arguments[ProgramOptions.Arguments];
            exitCode = engine.ExecuteFile(filePath, fileArguments);
            return true;
        }

        // 'check' command.
        if ((bool)arguments[ProgramOptions.Check])
        {
            exitCode = engine.CheckToolkit() ? 0 : 1;
            return true;
        }

        // 'list' command.
        if ((bool)arguments[ProgramOptions.List])
        {
            engine.ListToolkits();
            exitCode = 0;
            return true;
        }

        exitCode = default;
        return false;
    }
}
