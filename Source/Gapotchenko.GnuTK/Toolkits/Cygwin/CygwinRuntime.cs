// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.GnuTK.Helpers;

#pragma warning disable CA1822 // Mark members as static

namespace Gapotchenko.GnuTK.Toolkits.Cygwin;

/// <summary>
/// Provides Cygwin runtime functionality.
/// </summary>
/// <param name="getToolPath">
/// The function that returns a path of the specified Cygwin tool (<c>cygpath.exe</c>, <c>cygcheck.exe</c>, etc).
/// </param>
class CygwinRuntime(Func<string, string> getToolPath) :
    ToolkitRuntime
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
        List<string> args = ["-u"];
        AddCygpathOptionArgs(args, options);
        return Cygpath(args, path);
    }

    public override string ConvertPathToHostFormat(string path, ToolkitPathConversionOptions options)
    {
        List<string> args = ["-w"];
        AddCygpathOptionArgs(args, options);
        return Cygpath(args, path);
    }

    static void AddCygpathOptionArgs(IList<string> args, ToolkitPathConversionOptions options)
    {
        if (options.HasFlag(ToolkitPathConversionOptions.Absolute))
            args.Add("-a");
    }

    string Cygpath(IEnumerable<string> args, string path)
    {
        if (path is [])
            return path;

        string toolPath = getToolPath("cygpath.exe");

        var psi = new ProcessStartInfo
        {
            FileName = toolPath
        };

        var toolArgs = psi.ArgumentList;
        toolArgs.AddRange(args);
        toolArgs.Add(path);

        var output = new StringWriter();

        int exitCode = ProcessHelper.Execute(psi, output);
        if (exitCode != 0)
            return path;

        return output.ToString();
    }
}
