// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits;

/// <summary>
/// Represents a GNU toolkit consisting of multiple underlying toolkits.
/// </summary>
sealed class MultiplexToolkit(IScriptableToolkit scriptableToolkit, IEnumerable<IToolkitEnvironment> toolkitEnvironments) :
    IScriptableToolkit,
    IToolkitEnvironment
{
    public string Name
    {
        get =>
            field ??=
            string.Join(
                ',',
                GetUnderlyingToolkits().Select(toolkit => toolkit.Name));
        init;
    }

    public string Description
    {
        get =>
            field ??=
            string.Format(
                "Union of {0} toolkits.",
                LinguisticServices.CombineWithAnd(
                    GetUnderlyingToolkits()
                    .Select(toolkit => LinguisticServices.SingleQuote(toolkit.Description.TrimEnd('.')))));
        init;
    }

    public string? InstallationPath
    {
        get => field ?? scriptableToolkit.InstallationPath;
        init;
    }

    public int ExecuteCommand(string command, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment)
    {
        return scriptableToolkit.ExecuteCommand(
            command,
            arguments,
            GetCombinedEnvironment(environment));
    }

    public int ExecuteFile(string path, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string?>? environment)
    {
        return scriptableToolkit.ExecuteFile(
            path,
            arguments,
            GetCombinedEnvironment(environment));
    }

    IReadOnlyDictionary<string, string?>? GetCombinedEnvironment(IReadOnlyDictionary<string, string?>? environment) =>
        m_ToolkitEnvironments
        .Select(x => x.Environment)
        .Reverse()
        .Append(environment)
        .Aggregate(ToolkitKit.CombineEnvironments);

    public IReadOnlyDictionary<string, string?>? Environment =>
        GetUnderlyingToolkits()
        .OfType<IToolkitEnvironment>()
        .Select(x => x.Environment)
        .Reverse()
        .Aggregate(ToolkitKit.CombineEnvironments);

    public IToolkitFamily Family
    {
        get =>
            field ??=
            new MultiplexFamily(
                GetUnderlyingToolkits()
                .Select(toolkit => toolkit.Family.Traits)
                .Aggregate((a, b) => a | b) &
                ~ToolkitFamilyTraits.DeploymentMask);
        init;
    }

    sealed class MultiplexFamily(ToolkitFamilyTraits traits) : IToolkitFamily
    {
        public string Name => "Multiplex";
        public ToolkitFamilyTraits Traits => traits;
        public IEnumerable<IToolkit> EnumerateInstalledToolkits() => [];
        public IEnumerable<IToolkit> EnumerateToolkitsFromDirectory(string path) => [];
    }

    IEnumerable<IToolkit> GetUnderlyingToolkits() => [.. m_ToolkitEnvironments, scriptableToolkit];

    readonly IReadOnlyList<IToolkitEnvironment> m_ToolkitEnvironments = [.. toolkitEnvironments];
}
