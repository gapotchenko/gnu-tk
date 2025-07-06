// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits.System;

sealed class SystemToolkitFamily : IToolkitFamily
{
    public static IToolkitFamily Instance { get; } = new SystemToolkitFamily();

    SystemToolkitFamily()
    {
    }

    public string Name => "System";

    public ToolkitFamilyTraits Traits =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? ToolkitFamilyTraits.None
            : ToolkitFamilyTraits.Alike;

    public IEnumerable<IToolkit> EnumerateInstalledToolkits() => m_CachedInstalledToolkits ??= [new SystemToolkit(this)];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IToolkit>? m_CachedInstalledToolkits;

    public IEnumerable<IToolkit> EnumerateToolkitsFromDirectory(string path) => [];
}
