// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Diagnostics;
using Gapotchenko.FX.IO;
using Gapotchenko.GnuTK.Diagnostics;

namespace Gapotchenko.GnuTK.Cli;

static class CliVerbatim
{
    public static string GetCommandToExecute(IReadOnlyList<string> commandLineArguments)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // On Windows, the command line is natively represented as a single string.
            // Use the original command-line string directly to extract the required information.
            // Reconstructing it from parsed arguments could alter quoting or escaping and lead to subtle inaccuracies,
            // which goes against being verbatim.
            return ExtractCommandToExecute(Environment.CommandLine).ToString();
        }
        else
        {
            // Build a command from the specified command-line arguments.
            return CommandLine.Build(commandLineArguments);
        }
    }

    /// <summary>
    /// Extracts a command to execute from the specified command line string.
    /// </summary>
    static ReadOnlySpan<char> ExtractCommandToExecute(string commandLine)
    {
        var reader = new PositionTrackingTextReader(new StringReader(commandLine));
        using var enumerator = CommandLine.Split(reader).GetEnumerator();

        var state = CommandExtractionState.Argument; // skip the name of executable
        int j = -1;

        // Interpret command line using a state machine.
        while (enumerator.MoveNext())
        {
            switch (state)
            {
                case CommandExtractionState.Option:
                    switch (enumerator.Current)
                    {
                        case CliOptions.Verbatim:
                        case CliOptions.Strict or CliOptions.Shorthands.Strict:
                        case CliOptions.Integrated or CliOptions.Shorthands.Integrated:
                        case CliOptions.Posix or CliOptions.Shorthands.Posix:
                        case "-sp": // strict + posix
                        case "-si": // strict + integrated
                        case "-sip": // strict + integrated + posix
                        case "-ip": // integrated + posix
                            break;

                        case CliOptions.Toolkit or CliOptions.Shorthands.Toolkit:
                            state = CommandExtractionState.Argument;
                            break;

                        case CliOptions.ExecuteShellCommandLine or CliOptions.Shorthands.ExecuteShellCommandLine:
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

        throw new InternalException("Cannot extract a verbatim command from the command line.");
    }

    enum CommandExtractionState
    {
        Option,
        Argument,
        CaptureCommandLine
    }
}
