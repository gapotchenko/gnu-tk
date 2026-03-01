// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Text;

#pragma warning disable CA1822 // Mark members as static

namespace Gapotchenko.GnuTK.Toolkits.Cygwin;

class CygwinRuntime : ToolkitRuntime
{
    /// <summary>
    /// Adjusts a command-line argument before passing it to a Cygwin-based
    /// toolkit, correcting distortions introduced by Cygwin.
    /// </summary>
    /// <param name="value">The original argument value.</param>
    /// <returns>The corrected argument value</returns>
    /// <remarks>
    /// <para>
    /// Cygwin and related toolchains alter the handling of the <c>\</c>
    /// character in command-line arguments. While this usually has no effect,
    /// the loss of semantic precision can break certain scenarios.
    /// </para>
    /// <para>
    /// Reasons of that behavior are not well understood and remain a mystery.
    /// </para>
    /// </remarks>
    public string AdjustProgramArgument(string value)
    {
        return value
            // "Undo" the distortions made by Cygwin.
            .Replace(@"\", @"\\", StringComparison.Ordinal);
    }

    public override string ConvertPathToGuestFormat(string path, ToolkitPathConversionOptions options)
    {
        // TODO
        // Deduct the prefix from '/etc/fstab' file as described at
        // https://conemu.github.io/en/CygwinStartDir.html#cygdrive

        if (path.Length >= 2 && path[1] == ':' && char.IsAsciiLetter(path[0]))
        {
            // C:/Folder/File.txt => {prefix}/c/Folder/File.txt

            var builder = new StringBuilder(PathPrefix);

            // The drive letter.
            builder.Append('/').Append(char.ToLowerInvariant(path[0]));

            // The drive path.
            var s = path[2..].Replace('\\', '/').AsSpan().TrimStart('/');
            if (!s.IsEmpty)
                builder.Append('/').Append(s);

            return builder.ToString();
        }
        else
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }
    }

    public string? PathPrefix { get; init; }
}
