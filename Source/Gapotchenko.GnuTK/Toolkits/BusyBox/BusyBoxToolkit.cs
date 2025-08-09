// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.GnuTK.Helpers;
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
        return ExecuteShell(["-e", "-o", "pipefail", "-c", command, .. arguments], environment);
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return ExecuteShell([path, .. arguments], environment);
    }

    int ExecuteShell(
        IEnumerable<string> arguments,
        IReadOnlyDictionary<string, string?>? environment)
    {
        string busyboxPath = setupInstance.ResolvePath(setupInstance.ProductPath);

        var psi = new ProcessStartInfo
        {
            FileName = busyboxPath
        };

        var processEnvironment = psi.Environment;
        ToolkitEnvironment.CombineWith(processEnvironment, environment);

        var busyboxArguments = psi.ArgumentList;
        busyboxArguments.Add("sh");
        busyboxArguments.AddRange(arguments);

        return ProcessHelper.Execute(psi);
    }

    public string TranslateFilePath(string path) => path.Replace(Path.DirectorySeparatorChar, '/');
}
