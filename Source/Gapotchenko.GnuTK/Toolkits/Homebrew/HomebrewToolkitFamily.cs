// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits.Homebrew;

/// <summary>
/// Describes a family of GNU toolkits provided by Homebrew package manager.
/// </summary>
/// <remarks>
/// More information: <seealso href="https://brew.sh/"/>
/// </remarks>
sealed class HomebrewToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new HomebrewToolkitFamily();

    public string Name => "Homebrew";

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.None;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits()
    {
        // TODO

        return [];
    }

    public IEnumerable<IToolkit> EnumerateToolkitsFromDirectory(string path) => [];
}
