// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Console;
using Gapotchenko.GnuTK.Diagnostics;

namespace Gapotchenko.GnuTK.UI;

static class UIStyles
{
    public static void ErrorPrologue(TextWriter textWriter, DiagnosticCode? errorCode)
    {
        textWriter.Write(Invariant($"Error: {DiagnosticServices.GetErrorIdentifier(errorCode)}: "));
    }

    public readonly struct Scope : IDisposable
    {
        public static Scope Title(TextWriter textWriter) =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new(textWriter, bold: true)
                : new(textWriter, foregroundColor: ConsoleColor.White);

        public static Scope Success(TextWriter textWriter) => new(textWriter, foregroundColor: ConsoleColor.Green);

        public static Scope Error(TextWriter textWriter) => new(textWriter, foregroundColor: ConsoleColor.Red);

        Scope(TextWriter textWriter, ConsoleColor? foregroundColor = null, bool bold = false)
        {
            if (!ConsoleTraits.IsColorEnabled)
                return;

            if (bold)
            {
                textWriter.Write("\e[1m");
                m_TextWriter = textWriter;
            }

            if (foregroundColor is { } fgColor)
            {
                m_SavedForegroundColor = Console.ForegroundColor;
                Console.ForegroundColor = fgColor;
            }
        }

        public void Dispose()
        {
            if (m_SavedForegroundColor is { } fgColor)
                Console.ForegroundColor = fgColor;

            if (m_TextWriter is { } textWriter)
                textWriter.Write("\e[0m");
        }

        readonly ConsoleColor? m_SavedForegroundColor;
        readonly TextWriter? m_TextWriter;
    }
}
