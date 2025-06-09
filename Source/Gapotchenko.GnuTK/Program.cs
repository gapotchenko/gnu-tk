// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX;
using Gapotchenko.FX.AppModel;
using Gapotchenko.FX.Console;

namespace Gapotchenko.GnuTK;

static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        try
        {
            return Run(args);
        }
        catch (ProgramExitException e)
        {
            return e.ExitCode;
        }
        catch (Exception e)
        {
            bool useColor = ConsoleTraits.IsColorEnabled;
            if (useColor)
                Console.ForegroundColor = ConsoleColor.Red;
            var writer = Console.Error;
            writer.Write("Error");

            int? errorCode = (e as GnuTKClassifiedException)?.ErrorCode;
            writer.Write($": GNUTK{errorCode:D4}: ");

            writer.Write(e.Message);
            if (useColor)
                Console.ResetColor();
            writer.WriteLine();
            return 1;
        }
    }

    static int Run(string[] args)
    {
        CliServices.InitializeConsole();

        string usage =
            """
            Usage:
              gnu-tk [-t <name>] -c <command>
              gnu-tk [-t <name>] -l [--] <command>...
              gnu-tk [-t <name>] -f <file>
              gnu-tk toolkit (list | check [-t <name>]) [-q]
              gnu-tk (-h | --help) [-q]
              gnu-tk --version

            Options:
              -h --help            Show this help.
              --version            Show version.            
              -c --command         Execute a command using a GNU toolkit.
              -l --command-line    Execute a command line using a GNU toolkit.
              -f --file            Execute a file using a GNU toolkit.
              -t --toolkit=<name>  Use the specified GNU toolkit [default: auto].
              -q --quiet           Do not print any informational or diagnostic messages.
            
            Commands:
              toolkit list         List available GNU toolkits.
              toolkit check        Check a GNU toolkit.
            """;

        var arguments =
            CliServices.TryParseArguments(CanonicalizeArgs(args), usage) ??
            throw new GnuTKClassifiedException("Invalid program arguments.") { ErrorCode = 1 };

        if ((bool)arguments[ProgramOptions.Help])
        {
            if (!(bool)arguments[ProgramOptions.Quiet])
            {
                ShowLogo();
                Console.WriteLine();
            }
            Console.WriteLine(usage);
            return 1;
        }

        return Run(arguments);
    }

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

        var state = ArgsState.Begin;
        for (int i = 0; i < n; ++i)
        {
            string arg = args[i];

            switch (state)
            {
                case ArgsState.Begin:
                    switch (arg)
                    {
                        case "-?":
                        case "/?" when windows:
                            arg = ProgramOptions.Help;
                            state = ArgsState.End;
                            break;

                        case ProgramOptions.ExecuteCommandLine or "-l":
                            state = ArgsState.StartPositional;
                            break;

                        case ProgramOptions.ExecuteCommand or "-c":
                        case ProgramOptions.ExecuteFile or "-f":
                            state = ArgsState.End;
                            break;
                    }
                    break;

                case ArgsState.StartPositional:
                    if (arg != "--")
                    {
                        // Ensures that this and the next arguments will not be parsed as named parameters.
                        newArgs.Add("--");
                    }
                    state = ArgsState.End;
                    break;

                case ArgsState.End:
                    break;
            }

            newArgs.Add(arg);
        }

        return newArgs;
    }

    enum ArgsState
    {
        Begin,
        StartPositional,
        End
    }

    static void ShowLogo()
    {
        var appInfo = AppInformation.Current;

        Console.Write(appInfo.ProductName);
        Console.Write(" ");
        Console.WriteLine(GetAppVersion(appInfo));

        if (appInfo.Copyright is not null and var copyright)
            Console.WriteLine(CliServices.AdaptConsoleText(copyright));

        Console.WriteLine();
        Console.WriteLine("Provides seamless access to GNU toolkits on non-GNU operating systems.");
    }

    static int Run(IReadOnlyDictionary<string, object> arguments)
    {
        if ((bool)arguments[ProgramOptions.Version])
        {
            ShowVersion();
            return 0;
        }

        foreach (var i in arguments)
            Console.WriteLine("{0} = {1}", i.Key, i.Value);

        return 0;
    }

    static void ShowVersion()
    {
        Console.WriteLine(GetAppVersion(AppInformation.Current));
    }

    static string GetAppVersion(IAppInformation appInfo)
    {
        string version = appInfo.InformationalVersion;
        // Remove version metadata.
        if (version.LastIndexOf('+') is not -1 and var j)
            version = version[..j];
        return version;
    }
}
