// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Math.Intervals;
using Gapotchenko.Shields.Cygwin.Deployment;
using Gapotchenko.Shields.Homebrew.Deployment;
using Gapotchenko.Shields.Microsoft.Wsl.Deployment;
using Gapotchenko.Shields.MSys2.Deployment;

namespace Gapotchenko.GnuTK.Tests;

static class TestServices
{
    /// <summary>
    /// Enumerates installed GNU toolkits to test.
    /// </summary>
    public static IEnumerable<string> EnumerateToolkits()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (MSys2Deployment.EnumerateSetupInstances().Any())
                yield return "msys2";
            if (CygwinDeployment.EnumerateSetupInstances().Any())
                yield return "cygwin";
            if (WslDeployment.EnumerateSetupInstances(ValueInterval.FromInclusive(new Version(2, 0))).Any())
                yield return "wsl";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            if (BrewDeployment.EnumerateSetupInstances().Any())
                yield return "brew";
            yield return "system";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            yield return "system";
        }
    }
}
