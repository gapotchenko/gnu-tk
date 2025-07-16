// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Text;

namespace Gapotchenko.GnuTK.Toolkits.Cygwin;

static class CygwinFileSystem
{
    public static string TranslateFilePath(string path, string? prefix)
    {
        // TODO
        // Deduct the prefix from '/etc/fstab' file as described at
        // https://conemu.github.io/en/CygwinStartDir.html#cygdrive

        if (path.Length >= 2 && path[1] == ':' && char.IsAsciiLetter(path[0]))
        {
            // C:/Folder/File.txt => {prefix}/c/Folder/File.txt

            var builder = new StringBuilder(prefix);

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
            return path;
        }
    }
}
