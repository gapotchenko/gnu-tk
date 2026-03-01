// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Provides native GNU toolkit runtime functionality.
/// </summary>
sealed class NativeToolkitRuntime : ToolkitRuntime
{
    public static NativeToolkitRuntime Instance { get; } = new();

    NativeToolkitRuntime()
    {
    }
}
