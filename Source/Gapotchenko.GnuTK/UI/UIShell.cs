// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX;
using Gapotchenko.FX.AppModel;
using Gapotchenko.GnuTK.Diagnostics;
using System.Reflection;

namespace Gapotchenko.GnuTK.UI;

static class UIShell
{
    public static bool Run(
        IReadOnlyDictionary<string, object> arguments,
        string usage,
        out int exitCode)
    {
        if ((bool)arguments[ProgramOptions.Help])
        {
            if (!(bool)arguments[ProgramOptions.Quiet])
            {
                ShowLogo();
                Console.WriteLine();
            }
            Console.WriteLine(usage.Replace(" [--] ", " "));
            exitCode = 0;
            return true;
        }

        if ((bool)arguments[ProgramOptions.Version])
        {
            ShowVersion((bool)arguments[ProgramOptions.Quiet]);
            exitCode = 0;
            return true;
        }

        exitCode = default;
        return false;
    }

    static void ShowLogo()
    {
        var appInfo = GetAppInfo();

        Console.Write(appInfo.ProductName);
        Console.Write(' ');
        Console.Out.WriteLine(GetAppVersion(appInfo));

        if (appInfo.Copyright is not null and var copyright)
            Console.WriteLine(CliServices.AdaptConsoleText(copyright));

        Console.WriteLine();
        Console.WriteLine("Provides seamless access to GNU tools on non-GNU operating systems.");
    }

    static void ShowVersion(bool quiet)
    {
        var appInfo = GetAppInfo();

        if (!quiet)
        {
            Console.Write(appInfo.ProductName);
            Console.Write(' ');
        }

        Console.Out.WriteLine(GetAppVersion(appInfo));
    }

    static ReadOnlySpan<char> GetAppVersion(IAppInformation appInfo)
    {
        var version = appInfo.InformationalVersion.AsSpan();
        // Remove version's metadata.
        if (version.LastIndexOf('+') is not -1 and var j)
            version = version[..j];
        return version;
    }

    static IAppInformation GetAppInfo() => AppInformation.Current;

    public static void ShowError(Exception exception)
    {
        var writer = Console.Error;
        using (UIStyles.Scope.Error(writer))
        {
            writer.Write("Error");

            int? errorCode = (int?)(exception.SelfAndInnerExceptions().OfType<DiagnosticException>().FirstOrDefault()?.Code);
            writer.Write(Invariant($": GNUTK{errorCode:D4}: "));

            if (exception is InternalException || errorCode is null && exception is not ProgramException)
                writer.Write("Internal error: ");

            bool hasParent = false;

            bool point = false;
            foreach (var i in exception.SelfAndInnerExceptions())
            {
                if (i is TypeInitializationException or TargetInvocationException)
                    continue;

                string message = i.Message;

                var s = message.AsSpan().TrimEnd('.');
                if (s is [])
                    continue;
                point = s.Length != message.Length;

                if (hasParent)
                    writer.Write(" --> ");
                else
                    hasParent = true;

                writer.Write(s);
            }

            if (point)
                writer.Write('.');
        }
        writer.WriteLine();
    }
}
