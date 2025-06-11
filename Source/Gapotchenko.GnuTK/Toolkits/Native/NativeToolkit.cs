// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits.Native;

sealed class NativeToolkit(NativeToolkitFamily family) : IToolkit
{
    public string Name => "native";

    public string Description =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? "Native GNU system"
            : "Native GNU-like system";

    public string? InstallationPath => "/";

    public IToolkitFamily Family => family;

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments) => throw new NotImplementedException();
}
