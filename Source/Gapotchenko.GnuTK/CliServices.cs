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
        // Upgrade to Unicode on operating systems that come with it being tuned off.
        ConsoleEx.EnableUnicode();
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
        catch
        {
            parsedArgs = null;
        }

        if (parsedArgs is null)
            return null;

        // Translate the result to a canonical representation.
        return (IReadOnlyDictionary<string, object>)TranslateValue(parsedArgs);

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
                return dictionary;
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
}
