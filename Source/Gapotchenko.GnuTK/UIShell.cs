// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.AppModel;
using Gapotchenko.FX.Console;
using Gapotchenko.GnuTK.Diagnostics;

namespace Gapotchenko.GnuTK;

static class UIShell
{
    public static bool Run(IReadOnlyDictionary<string, object> arguments, string usage, out int exitCode)
    {
        if ((bool)arguments[ProgramOptions.Help])
        {
            if (!(bool)arguments[ProgramOptions.Quiet])
            {
                ShowLogo();
                Console.WriteLine();
            }
            Console.WriteLine(usage.Replace(" [--] ", " "));
            exitCode = 1;
            return true;
        }
        else if ((bool)arguments[ProgramOptions.Version])
        {
            ShowVersion();
            exitCode = 0;
            return true;
        }
        else
        {
            exitCode = default;
            return false;
        }
    }

    static void ShowLogo()
    {
        var appInfo = AppInformation.Current;

        Console.Write(appInfo.ProductName);
        Console.Write(' ');
        Console.Out.WriteLine(GetAppVersion(appInfo));

        if (appInfo.Copyright is not null and var copyright)
            Console.WriteLine(CliServices.AdaptConsoleText(copyright));

        Console.WriteLine();
        Console.WriteLine("Provides seamless access to GNU toolkits on non-GNU operating systems.");
    }

    static void ShowVersion()
    {
        Console.Out.WriteLine(GetAppVersion(AppInformation.Current));
    }

    static ReadOnlySpan<char> GetAppVersion(IAppInformation appInfo)
    {
        var version = appInfo.InformationalVersion.AsSpan();
        // Remove version's metadata.
        if (version.LastIndexOf('+') is not -1 and var j)
            version = version[..j];
        return version;
    }

    public static void ShowError(Exception exception)
    {
        bool useColor = ConsoleTraits.IsColorEnabled;
        if (useColor)
            Console.ForegroundColor = ConsoleColor.Red;
        var writer = Console.Error;
        writer.Write("Error");

        int? errorCode = (int?)(exception as DiagnosticException)?.Code;
        writer.Write(Invariant($": GNUTK{errorCode:D4}: "));

        writer.Write(exception.Message);
        if (useColor)
            Console.ResetColor();
        writer.WriteLine();
    }
}
