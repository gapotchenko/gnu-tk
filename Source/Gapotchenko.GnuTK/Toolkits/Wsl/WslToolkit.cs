// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.Shields.Microsoft.Wsl.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.Wsl;

sealed class WslToolkit(WslToolkitFamily family, IWslSetupInstance setupInstance) : IToolkit
{
    public string Name => "wsl";

    public string Description => setupInstance.DisplayName;

    public string? InstallationPath => setupInstance.InstallationPath;

    public IToolkitFamily Family => family;

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments) => throw new NotImplementedException();

    public int ExecuteFile(string path, IReadOnlyList<string> arguments) => throw new NotImplementedException();
}
