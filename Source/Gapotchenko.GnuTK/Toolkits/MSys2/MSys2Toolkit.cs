// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.Shields.MSys2.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.MSys2;

sealed class MSys2Toolkit(MSys2ToolkitFamily family, IMSys2Environment environment) : IToolkit
{
    public string Name => field ??= $"{family.Name}-{environment.Name}".ToLowerInvariant();

    public string Description => environment.SetupInstance.DisplayName;

    public string? InstallationPath => environment.SetupInstance.InstallationPath;

    public IToolkitFamily Family => family;

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments) => throw new NotImplementedException();

    public int ExecuteFile(string path, IReadOnlyList<string> arguments) => throw new NotImplementedException();
}
