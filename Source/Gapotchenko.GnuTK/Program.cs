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
              gnu-tk [-t <name>] -c <command>
              gnu-tk [-t <name>] -l [--] <command>...
              gnu-tk [-t <name>] -f <file>
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
            throw new GnuTKDiagnosticException("Invalid program arguments.") { ErrorCode = DiagnosticErrorCodes.InvalidProgramArguments };

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

                        case ProgramOptions.ExecuteCommandLine or "-l":
                            state = ArgsCanonicalizationState.StartPositional;
                            break;

                        case ProgramOptions.ExecuteCommand or "-c":
                        case ProgramOptions.ExecuteFile or "-f":
                            state = ArgsCanonicalizationState.End;
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
        var engine = new Engine()
        {
            ToolkitName =
                NormalizeToolkitName((string)arguments[ProgramOptions.Toolkit])
                ?? NormalizeToolkitName(Environment.GetEnvironmentVariable("GNU_TK_TOOLKIT")),
            ToolkitPaths = GetToolkitPaths(),
            Quiet = (bool)arguments[ProgramOptions.Quiet]
        };

        if ((bool)arguments[ProgramOptions.List])
        {
            engine.ListToolkits();
            return 0;
        }

        CliServices.DumpArguments(arguments);

        return 1;
    }

    static string? NormalizeToolkitName(string? name) =>
        Empty.Nullify(
            Empty.Nullify(name),
            "auto",
            StringComparison.OrdinalIgnoreCase);

    static IReadOnlyList<string> GetToolkitPaths()
    {
        return
            Environment.GetEnvironmentVariable("GNU_TK_TOOLKIT_PATH")
            ?.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            ?? [];
    }
}
