// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.GnuTK.Toolkits.MSys2;

namespace Gapotchenko.GnuTK.Toolkits.Git;

sealed class GitRuntime(Func<string, string> getToolPath) :
    MSys2Runtime(getToolPath)
{
}
