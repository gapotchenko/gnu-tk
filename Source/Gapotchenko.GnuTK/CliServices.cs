// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using DocoptNet;
using Gapotchenko.FX.Console;
using Gapotchenko.FX.Text;
using System.Collections;

namespace Gapotchenko.GnuTK;

/// <summary>
/// Provides services for command-line interface.
/// </summary>
static class CliServices
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
    public static string? AdaptConsoleText(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        if (!Console.OutputEncoding.IsUnicodeScheme)
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
}
