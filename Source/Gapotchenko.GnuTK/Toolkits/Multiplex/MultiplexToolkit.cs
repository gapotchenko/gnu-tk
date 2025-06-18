// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Toolkits.Multiplex;

/// <summary>
/// Represents a GNU toolkit consisting of multiple underlying toolkits.
/// </summary>
sealed class MultiplexToolkit(IScriptableToolkit scriptableToolkit, IEnumerable<IToolkitEnvironment> toolkitEnvironments) :
    IToolkit,
    IToolkitFamily,
    IScriptableToolkit,
    IToolkitEnvironment
{
    public string Name => string.Join(
        ',',
        GetUnderlyingToolkits().Select(toolkit => toolkit.Name));

    public string Description => string.Format(
        "Union of {0} toolkits.",
        LinguisticServices.CombineWithAnd(
            GetUnderlyingToolkits()
            .Select(toolkit => LinguisticServices.SingleQuote(toolkit.Description.TrimEnd('.')))));

    public string? InstallationPath => scriptableToolkit.InstallationPath;

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

    public IToolkitFamily Family => this;

    #region IToolkitFamily

    string IToolkitFamily.Name => "Multiplex";

    ToolkitFamilyTraits IToolkitFamily.Traits =>
        GetUnderlyingToolkits()
        .Select(toolkit => toolkit.Family.Traits)
        .Aggregate((a, b) => a | b) &
        ~ToolkitFamilyTraits.DeploymentMask;

    IEnumerable<IToolkit> IToolkitFamily.EnumerateInstalledToolkits() => [];

    IEnumerable<IToolkit> IToolkitFamily.EnumerateToolkitsFromDirectory(string path) => [];

    #endregion

    IEnumerable<IToolkit> GetUnderlyingToolkits() => [.. m_ToolkitEnvironments, scriptableToolkit];

    readonly IReadOnlyList<IToolkitEnvironment> m_ToolkitEnvironments = [.. toolkitEnvironments];
}
