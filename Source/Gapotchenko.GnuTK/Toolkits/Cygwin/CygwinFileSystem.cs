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
        if (path.Length >= 2 && path[1] == ':' && char.IsAsciiLetter(path[0]))
        {
            // C:/Folder/File.txt => /c/Folder/File.txt

            var builder = new StringBuilder(prefix);
            builder.Append('/').Append(char.ToLowerInvariant(path[0]));

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
