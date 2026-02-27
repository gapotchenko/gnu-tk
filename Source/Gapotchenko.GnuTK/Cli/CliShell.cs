// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using DocoptNet;
using Gapotchenko.FX;
using Gapotchenko.FX.AppModel;
using Gapotchenko.FX.Console;
using Gapotchenko.FX.Text;
using Gapotchenko.GnuTK.Diagnostics;
using System.Collections;
using System.Reflection;

namespace Gapotchenko.GnuTK.Cli;

/// <summary>
/// Provides services for command-line interface.
/// </summary>
static class CliShell
{
    public static void InitializeConsole()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Prefer Unicode if possible.
            ConsoleEx.EnableUnicode();
        }
    }

    /// <summary>
    /// Adapts the specified text to the capabilities of the console.
    /// </summary>
    /// <param name="text">The text to adapt.</param>
    /// <returns>The adapted text.</returns>
    [return: NotNullIfNotNull(nameof(text))]
    static string? AdaptConsoleText(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // A workaround for the issue that appeared since .NET 10.0 SDK (v10.0.100-preview.6).
        if (!EncodingPolyfills.get_IsUnicodeScheme(Console.OutputEncoding))
        {
            text = text
                .Replace("©", "(C)", StringComparison.Ordinal)
                .Replace("®", "(R)", StringComparison.Ordinal);
        }

        return text;
    }

    public static IReadOnlyDictionary<string, object>? TryParseArguments(IEnumerable<string> arguments, string documentation)
    {
        IDictionary<string, ValueObject>? parsedArgs;
        try
        {
            parsedArgs = new Docopt().Apply(
                documentation,
                arguments as ICollection<string> ?? [.. arguments],
                help: false);
        }
        catch (DocoptInputErrorException)
        {
            parsedArgs = null;
        }

        if (parsedArgs is null)
            return null;

        return (IReadOnlyDictionary<string, object>)TranslateValue(parsedArgs);

        // Translates a value to the canonical immutable representation.
        [return: NotNullIfNotNull(nameof(value))]
        static object? TranslateValue(object? value)
        {
            if (value is IDictionary<string, ValueObject> valueDictionary)
            {
                var dictionary = new Dictionary<string, object>(valueDictionary.Count, StringComparer.Ordinal);
                foreach (var i in valueDictionary)
                {
                    if (TranslateValue(i.Value.Value) is not null and var v)
                        dictionary[i.Key] = v;
                }
                return dictionary.AsReadOnly();
            }
            else if (value is ArrayList arrayList)
            {
                IReadOnlyList<object?> list = [.. arrayList.Cast<object?>().Select(TranslateValue)];
                if (list.All(x => x is string))
                    return (IReadOnlyList<string>)[.. list.Cast<string>()];
                else
                    return list;
            }
            else
            {
                return value;
            }
        }
    }

    [Conditional("DEBUG")]
    public static void DumpArguments(IReadOnlyDictionary<string, object> arguments)
    {
        foreach (var i in arguments)
            Console.WriteLine("{0} = {1}", i.Key, i.Value);
    }

    public static bool Run(
        IReadOnlyDictionary<string, object> arguments,
        string usage,
        out int exitCode)
    {
        if ((bool)arguments[CliOptions.Help])
        {
            if (!(bool)arguments[CliOptions.Quiet])
            {
                ShowLogo();
                Console.WriteLine();
            }
            Console.WriteLine(usage.Replace(" [--] ", " "));
            exitCode = 0;
            return true;
        }

        if ((bool)arguments[CliOptions.Version])
        {
            ShowVersion((bool)arguments[CliOptions.Quiet]);
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
            Console.WriteLine(AdaptConsoleText(copyright));

        Console.WriteLine();
        Console.WriteLine("Provides portable access to GNU toolkits.");
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
        using (CliStyles.Scope.Error(writer))
        {
            var errorCode = exception.SelfAndInnerExceptions().OfType<DiagnosticException>().FirstOrDefault()?.Code;
            CliStyles.ErrorPrologue(writer, errorCode);

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
