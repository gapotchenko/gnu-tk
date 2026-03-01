// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.GnuTK.Toolkits.Cygwin;

namespace Gapotchenko.GnuTK.Toolkits.MSys2;

/// <summary>
/// Provides MSYS2 toolkit runtime functionality.
/// </summary>
class MSys2Runtime(Func<string, string> getToolPath) :
    CygwinRuntime(getToolPath)
{
}
