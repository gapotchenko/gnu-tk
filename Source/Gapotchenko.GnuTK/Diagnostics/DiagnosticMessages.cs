// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.GnuTK.Helpers;

namespace Gapotchenko.GnuTK.Diagnostics;

static class DiagnosticMessages
{
    public static string SuitableToolkitNotFound(IReadOnlyList<string>? names) =>
        names is null
            ? DiagnosticResources.SuitableToolkitNotFound
            : string.Format(
                DiagnosticResources.SuitableToolkitXNotFound,
                LinguisticHelper.CombineWithOr(names.Select(LinguisticHelper.SingleQuote)));

    public static string SuitableStrictToolkitNotFound(IReadOnlyList<string>? names) =>
        names is null
            ? DiagnosticResources.SuitableStrictToolkitNotFound
            : string.Format(
                DiagnosticResources.SuitableStrictToolkitXNotFound,
                LinguisticHelper.CombineWithOr(names.Select(LinguisticHelper.SingleQuote)));

    public static string ModuleNotFound(string? name) => string.Format(DiagnosticResources.ModuleXNotFound, name);

    public static string CannotStartProcess(string? name) => string.Format(DiagnosticResources.CannotStartProcessX, name);

    public static string ConflictingProgramArguments(string? a, string? b) => string.Format(DiagnosticResources.ConflictingProgramArgumentsXY, a, b);

    public static string BuiltinToolkitDirectoryNotFound(string? toolkit, string? directory) => string.Format(DiagnosticResources.BuiltinToolkitXDirectoryYNotFound, toolkit, directory);

    public static string InvalidProgramArguments => DiagnosticResources.InvalidProgramArguments;

    public static string MissingProgramArguments => DiagnosticResources.MissingProgramArguments;

    public static string TryXForMoreInformation(string arg) => string.Format(DiagnosticResources.TryXForMoreInformation, arg);
}
