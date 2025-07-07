// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX;
using Gapotchenko.FX.Diagnostics;
using Gapotchenko.FX.IO;
using Gapotchenko.GnuTK.Diagnostics;
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
        string usage =
            """
            Usage:
              gnu-tk [-t <name>] [-s] [-p] -c [--] <command> [<argument>...]
              gnu-tk [-t <name>] [-s] [-p] -l [--] <argument>...
              gnu-tk [-t <name>] [-s] [-p] -f [--] <file> [<argument>...]
              gnu-tk (list | check [-t <name>]) [-s] [-q]
              gnu-tk (--help | --version) [-q]

            Options:
              --help               Show this help.
              --version            Show version.            
              -c --command         Execute a command using a GNU toolkit.
              -l --command-line    Execute a command line using a GNU toolkit.
              -f --file            Execute a file using a GNU toolkit.
              -t --toolkit=<name>  Use the specified GNU toolkit [default: auto].
              -s --strict          Use a toolkit with strict GNU semantics.
              -p --posix           Enables POSIX-compliant behavior.
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

    #region Program arguments canonicalization

    /// <summary>
    /// Interprets naturally occurring program arguments and
    /// rewrites them in canonical form suitable for the formal parsing.
    /// Additionally, validates arguments order.
    /// </summary>
    static IReadOnlyList<string> CanonicalizeArgs(IReadOnlyList<string> args)
    {
        int n = args.Count;
        if (n == 0)
            return [ProgramOptions.Help];

        var newArgs = new List<string>(n + 1);

        var state = ArgsCanonicalizationState.Option;
        for (int i = 0; i < n; ++i)
        {
            string arg = args[i];

            switch (state)
            {
                case ArgsCanonicalizationState.Option:
                    switch (arg)
                    {
                        case ProgramOptions.ExecuteCommand or ProgramOptions.Shorthands.ExecuteCommand:
                        case ProgramOptions.ExecuteCommandLine or ProgramOptions.Shorthands.ExecuteCommandLine:
                        case ProgramOptions.ExecuteFile or ProgramOptions.Shorthands.ExecuteFile:
                            state = ArgsCanonicalizationState.Positional;
                            break;

                        case ProgramOptions.Toolkit or ProgramOptions.Shorthands.Toolkit:
                            state = ArgsCanonicalizationState.Argument;
                            break;

                        case "-?" or "/?" when RuntimeInformation.IsOSPlatform(OSPlatform.Windows):
                            // A long-standing tradition on Windows.
                            arg = ProgramOptions.Help;
                            break;

                        case ProgramOptions.Help:
                        case ProgramOptions.Quiet or ProgramOptions.Shorthands.Quiet:
                        case ProgramOptions.Version:
                        case ProgramOptions.List:
                        case ProgramOptions.Check:
                        case ProgramOptions.Strict or ProgramOptions.Shorthands.Strict:
                        case ProgramOptions.Posix or ProgramOptions.Shorthands.Posix:
                        case "-sp" or "-ps": // strict + posix
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
            Strict = (bool)arguments[ProgramOptions.Strict],
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
    }

    static bool RunEngine(Engine engine, IReadOnlyDictionary<string, object> arguments, out int exitCode)
    {
        // Most frequently used options are handled first.

        // '-c' command-line option.
        if ((bool)arguments[ProgramOptions.ExecuteCommand])
        {
            string command = (string)arguments[ProgramOptions.Command];
            var commandArguments = (IReadOnlyList<string>)arguments[ProgramOptions.Arguments];
            exitCode = engine.ExecuteCommand(command, commandArguments);
            return true;
        }

        // '-l' command-line option.
        if ((bool)arguments[ProgramOptions.ExecuteCommandLine])
        {
            // In Windows OS, the native representation of a command line is a string.
            // Take a benefit of that fact by extracting command information directly.
            // Otherwise, it would be necessary to reconstruct it and this can lead to imprecisions.
            var command = ExtractCommand(Environment.CommandLine);
            exitCode = engine.ExecuteCommand(command.ToString(), []);
            return true;
        }

        // '-f' command-line option.
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

    /// <summary>
    /// Extracts a command from the specified command line.
    /// </summary>
    static ReadOnlySpan<char> ExtractCommand(string commandLine)
    {
        var reader = new PositionTrackingTextReader(new StringReader(commandLine));
        using var enumerator = CommandLine.Split(reader).GetEnumerator();

        var state = CommandExtractionState.Argument; // skip the name of executable
        int j = -1;

        // Interpret command-line arguments using a state machine.
        while (enumerator.MoveNext())
        {
            switch (state)
            {
                case CommandExtractionState.Option:
                    switch (enumerator.Current)
                    {
                        case ProgramOptions.Strict or ProgramOptions.Shorthands.Strict:
                        case ProgramOptions.Posix or ProgramOptions.Shorthands.Posix:
                        case "-sp" or "-ps": // strict + posix
                            break;

                        case ProgramOptions.Toolkit or ProgramOptions.Shorthands.Toolkit:
                            state = CommandExtractionState.Argument;
                            break;

                        case ProgramOptions.ExecuteCommandLine or ProgramOptions.Shorthands.ExecuteCommandLine:
                            j = (int)reader.Position;
                            state = CommandExtractionState.CaptureCommandLine;
                            break;

                        case var option:
                            throw new InternalException(string.Format("Cannot interpret command-line option '{0}'.", option));
                    }
                    break;

                case CommandExtractionState.Argument:
                    // Start interpreting command-line arguments.
                    state = CommandExtractionState.Option;
                    break;

                case CommandExtractionState.CaptureCommandLine:
                    Debug.Assert(j != -1);
                    // Skip an optional delimiter of positional arguments.
                    if (enumerator.Current == "--")
                        j = (int)reader.Position;
                    return commandLine.AsSpan(j);
            }
        }

        throw new InternalException("Cannot extract a command from the command line.");
    }

    enum CommandExtractionState
    {
        Option,
        Argument,
        CaptureCommandLine
    }
}
