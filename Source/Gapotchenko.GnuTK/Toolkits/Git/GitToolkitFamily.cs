// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.Shields.Git.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.Git;

/// <summary>
/// Describes a family of GNU toolkits provided by Git for Windows
/// distributed version control system.
/// </summary>
/// <remarks>
/// More information: <see href="https://git-scm.com/downloads/win"/>
/// </remarks>
sealed class GitToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new GitToolkitFamily();

    GitToolkitFamily()
    {
    }

    public string Name => "Git";

    public string? InformativeName => null;

    public IReadOnlyList<string> Aliases => [];

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.Installable | ToolkitFamilyTraits.Portable | ToolkitFamilyTraits.FilePathTranslation;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() =>
        GitDeployment.EnumerateSetupInstances()
        .Select(setupInstance => TryCreateToolkit(setupInstance, ToolkitTraits.None))
        .Where(toolkit => toolkit != null)!;

    public IEnumerable<IToolkit> EnumerateToolkitsInDirectory(string path, ToolkitTraits traits) =>
        GitSetupInstance.TryOpen(path) is { } setupInstance &&
        TryCreateToolkit(setupInstance, traits) is { } toolkit
            ? [toolkit]
            : [];

    GitToolkit? TryCreateToolkit(IGitSetupInstance setupInstance, ToolkitTraits traits)
    {
        string shellPath = setupInstance.ResolvePath(@"usr\bin\sh.exe");
        if (!File.Exists(shellPath))
            return null;

        return new GitToolkit(this, setupInstance, shellPath, traits);
    }
}
