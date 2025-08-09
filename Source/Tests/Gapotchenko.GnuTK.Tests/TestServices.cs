// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Math.Intervals;
using Gapotchenko.Shields.BusyBox.Deployment;
using Gapotchenko.Shields.Cygwin.Deployment;
using Gapotchenko.Shields.Git.Deployment;
using Gapotchenko.Shields.Homebrew.Deployment;
using Gapotchenko.Shields.Microsoft.Wsl.Deployment;
using Gapotchenko.Shields.MSys2.Deployment;

namespace Gapotchenko.GnuTK.Tests;

static class TestServices
{
    /// <summary>
    /// Enumerates installed GNU toolkits.
    /// </summary>
    public static IEnumerable<string> EnumerateToolkits()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (MSys2Deployment.EnumerateSetupInstances().Any())
                yield return "msys2";
            if (CygwinDeployment.EnumerateSetupInstances().Any())
                yield return "cygwin";
            if (GitDeployment.EnumerateSetupInstances().Any(instance => File.Exists(instance.ResolvePath(@"usr\bin\sh.exe"))))
                yield return "git";
            if (WslDeployment.EnumerateSetupInstances(ValueInterval.FromInclusive(new Version(2, 0))).Any())
                yield return "wsl";
            yield return "busybox"; // built-in
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            if (BrewDeployment.EnumerateSetupInstances().Any())
                yield return "brew";
            yield return "system";
            if (BusyBoxDeployment.EnumerateSetupInstances().Any())
                yield return "busybox";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            yield return "system";
            if (BusyBoxDeployment.EnumerateSetupInstances().Any())
                yield return "busybox";
        }
    }

    /// <summary>
    /// Gets the base directory path.
    /// </summary>
    public static string BasePath => field ??= GetAssemblyAndBaseDirectories().BasePath;

    /// <summary>
    /// Gets the file path of GnuTK tool.
    /// </summary>
    public static string ToolPath => field ??= GetToolPathCore();

    static string GetToolPathCore()
    {
        var (path, basePath) = GetAssemblyAndBaseDirectories();

        string tfm = Path.GetFileName(path);
        string? configuration =
            Path.GetFileName(Path.GetDirectoryName(path)) ??
            throw new InvalidOperationException("Cannot determine the assembly configuration.");

        string toolPath = Path.Combine(basePath, "Gapotchenko.GnuTK", "bin", configuration, tfm, "Gapotchenko.GnuTK");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            toolPath += ".exe";

        if (!File.Exists(toolPath))
            throw new FileNotFoundException("Tool path is not found.", toolPath);

        return toolPath;
    }

    static (string AssemblyDirectoryPath, string BasePath) GetAssemblyAndBaseDirectories()
    {
        string? path = Path.GetDirectoryName(typeof(TestServices).Assembly.Location);
        if (string.IsNullOrEmpty(path))
            throw new InvalidOperationException("Cannot determine the assembly directory.");

        string basePath = Path.GetFullPath(Path.Combine(path, "../../../../.."));

        return (path, basePath);
    }
}
