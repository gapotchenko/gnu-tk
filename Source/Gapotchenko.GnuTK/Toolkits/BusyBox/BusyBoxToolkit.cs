// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.Shields.BusyBox.Deployment;

namespace Gapotchenko.GnuTK.Toolkits.BusyBox;

sealed class BusyBoxToolkit(
    BusyBoxToolkitFamily family,
    IBusyBoxSetupInstance setupInstance,
    ToolkitTraits traits) :
    IScriptableToolkit
{
    public string Name => "busybox";

    public string Description => setupInstance.DisplayName;

    public string? InstallationPath => setupInstance.ResolvePath(setupInstance.ProductPath);

    public IToolkitFamily Family => family;

    public ToolkitTraits Traits => traits;

    public ToolkitIsolation Isolation => ToolkitIsolation.None;

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        throw new NotImplementedException();
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        throw new NotImplementedException();
    }

    public string TranslateFilePath(string path) => path.Replace(Path.DirectorySeparatorChar, '/');
}
