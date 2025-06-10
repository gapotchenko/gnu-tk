// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits.MSys2;

sealed class MSys2ToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new MSys2ToolkitFamily();

    MSys2ToolkitFamily()
    {
    }

    public string Name => "MSYS2";

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.Installable | ToolkitFamilyTraits.Portable;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() => []; // TODO

    public IEnumerable<IToolkit> EnumerateToolkitsFromDirectory(string path) => []; // TODO
}
