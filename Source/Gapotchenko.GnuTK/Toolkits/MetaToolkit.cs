// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Represents an amalgamation of GNU toolkits.
/// </summary>
sealed class MetaToolkit(IScriptableToolkit scriptableToolkit, IEnumerable<IToolkitEnvironment> toolkitEnvironments) :
    IScriptableToolkit,
    IToolkitEnvironment
{
    public string Name
    {
        get =>
            field ??=
            string.Join(
                '+',
                GetUnderlyingToolkits().Select(toolkit => toolkit.Name));
        init;
    }

    public string Description
    {
        get =>
            field ??=
            string.Format(
                "{0} meta toolkit.",
                string.Join(
                    " + ",
                    GetUnderlyingToolkits()
                    .Select(toolkit => LinguisticServices.SingleQuote(toolkit.Description.TrimEnd('.')))));
        init;
    }

    public string? InstallationPath
    {
        get => field ?? scriptableToolkit.InstallationPath;
        init;
    }

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return scriptableToolkit.ExecuteCommand(
            command,
            arguments,
            GetCombinedEnvironment(environment),
            options);
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment, ToolkitExecutionOptions options)
    {
        return scriptableToolkit.ExecuteFile(
            path,
            arguments,
            GetCombinedEnvironment(environment),
            options);
    }

    IReadOnlyDictionary<string, string?>? GetCombinedEnvironment(IReadOnlyDictionary<string, string?>? environment) =>
        m_ToolkitEnvironments
        .Select(x => x.Environment)
        .Reverse()
        .Append(environment)
        .Aggregate(ToolkitEnvironment.Combine);

    public IReadOnlyDictionary<string, string?>? Environment =>
        GetUnderlyingToolkits()
        .OfType<IToolkitEnvironment>()
        .Select(x => x.Environment)
        .Reverse()
        .Aggregate(ToolkitEnvironment.Combine);

    public IToolkitFamily Family
    {
        get =>
            field ??=
            new MetaFamily(
                GetUnderlyingToolkits()
                .Select(toolkit => toolkit.Family.Traits)
                .Aggregate((a, b) => a | b) &
                ~ToolkitFamilyTraits.DeploymentMask);
        init;
    }

    sealed class MetaFamily(ToolkitFamilyTraits traits) : IToolkitFamily
    {
        public string Name => "Meta";
        public IReadOnlyList<string> Aliases => [];
        public ToolkitFamilyTraits Traits => traits;
        public IEnumerable<IToolkit> EnumerateInstalledToolkits() => [];
        public IEnumerable<IToolkit> EnumerateToolkitsFromDirectory(string path) => [];
    }

    IEnumerable<IToolkit> GetUnderlyingToolkits() => [.. m_ToolkitEnvironments, scriptableToolkit];

    readonly IReadOnlyList<IToolkitEnvironment> m_ToolkitEnvironments = [.. toolkitEnvironments];

    public string TranslateFilePath(string path) => scriptableToolkit.TranslateFilePath(path);

    public ToolkitIsolation Isolation => scriptableToolkit.Isolation;
}
