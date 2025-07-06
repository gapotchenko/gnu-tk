// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Linq;
using Gapotchenko.Shields.Homebrew.Management;

namespace Gapotchenko.GnuTK.Toolkits.Homebrew;

class HomebrewToolkitEnvironment(
    HomebrewToolkitFamily family,
    IBrewPackageManagement packageManagement,
    IEnumerable<BrewPackage> packages) :
    IToolkitEnvironment
{
    public string Name => "brew";

    public string Description => "Homebrew packages.";

    public string? InstallationPath => PackageManagement.Manager.Setup.InstallationPath;

    public IToolkitFamily Family => family;

    public IReadOnlyDictionary<string, string?>? Environment => field ??= GetEnvironmentCore();

    IReadOnlyDictionary<string, string?> GetEnvironmentCore()
    {
        var environment = EnvironmentServices.CreateEnvironment();
        foreach (var package in m_Packages)
            EnvironmentServices.CombineEnvironmentWith(environment, GetPackageEnvironment(package));
        return environment;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly IEnumerable<BrewPackage> m_Packages = packages.Memoize();

    IReadOnlyDictionary<string, string?>? GetPackageEnvironment(BrewPackage package)
    {
        string? packagePath = PackageManagement.TryGetPackagePath(package);
        if (packagePath is null)
            return null;

        // ------------------------------------------------------------------

        string? path = null;

        string gnuBinPath = Path.Combine(packagePath, "libexec", "gnubin");
        if (Directory.Exists(gnuBinPath))
        {
            // Sample packages: coreutils, gnu-sed.
            path = gnuBinPath;
        }
        else
        {
            string binPath = Path.Combine(packagePath, "bin");
            if (Directory.Exists(binPath))
            {
                // Sample packages: binutils.
                path = binPath;
            }
        }

        // ------------------------------------------------------------------

        string? cppFlags = null;

        string includePath = Path.Combine(packagePath, "include");
        if (Directory.Exists(includePath))
        {
            if (Directory.EnumerateFiles(includePath, "*.h").Any())
            {
                // Sample packages: binutils.
                cppFlags = $"-I{includePath}";
            }
        }

        // ------------------------------------------------------------------

        string? ldFlags = null;

        string libPath = Path.Combine(packagePath, "lib");
        if (Directory.Exists(libPath))
        {
            if (Directory.EnumerateFiles(libPath, "*.a").Any())
            {
                // Sample packages: binutils.
                ldFlags = $"-L{libPath}";
            }
        }

        // ------------------------------------------------------------------

        var environment = EnvironmentServices.CreateEnvironment();
        if (path is not null)
            environment["PATH"] = path;
        if (cppFlags is not null)
            environment["CPPFLAGS"] = cppFlags;
        if (ldFlags is not null)
            environment["LDFLAGS"] = ldFlags;
        return environment;
    }

    protected IBrewPackageManagement PackageManagement => packageManagement;
}
