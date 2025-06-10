// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits.Native;

sealed class NativeToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new NativeToolkitFamily();

    NativeToolkitFamily()
    {
    }

    public string Name => "Native";

    public ToolkitFamilyTraits Traits => ToolkitFamilyTraits.None;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() => m_CachedInstalledToolkits ??= [new NativeToolkit(this)];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IToolkit>? m_CachedInstalledToolkits;

    public IEnumerable<IToolkit> EnumerateToolkitsFromDirectory(string path) => [];
}
