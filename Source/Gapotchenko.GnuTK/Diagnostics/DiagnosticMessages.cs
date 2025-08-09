// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Diagnostics;

static class DiagnosticMessages
{
    public static string SuitableToolkitNotFound(IReadOnlyList<string>? names) =>
        names is null
            ? DiagnosticResources.SuitableToolkitNotFound
            : string.Format(
                DiagnosticResources.SuitableToolkitXNotFound,
                LinguisticServices.CombineWithOr(names.Select(LinguisticServices.SingleQuote)));

    public static string SuitableStrictToolkitNotFound(IReadOnlyList<string>? names) =>
        names is null
            ? DiagnosticResources.SuitableStrictToolkitNotFound
            : string.Format(
                DiagnosticResources.SuitableStrictToolkitXNotFound,
                LinguisticServices.CombineWithOr(names.Select(LinguisticServices.SingleQuote)));

    public static string ModuleNotFound(string? name) => string.Format(DiagnosticResources.ModuleXNotFound, name);

    public static string CannotStartProcess(string? name) => string.Format(DiagnosticResources.CannotStartProcessX, name);

    public static string ConflictingProgramArguments(string? a, string? b) => string.Format(DiagnosticResources.ConflictingProgramArgumentsXY, a, b);

    public static string BuiltInToolkitDirectoryNotFound(string? toolkit, string? directory) => string.Format(DiagnosticResources.BuiltInToolkitXDirectoryYNotFound, toolkit, directory);
}
