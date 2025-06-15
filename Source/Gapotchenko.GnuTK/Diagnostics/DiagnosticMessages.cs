// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.GnuTK.Properties;

namespace Gapotchenko.GnuTK.Diagnostics;

static class DiagnosticMessages
{
    public static string SuitableToolkitNotFound(IReadOnlyList<string>? names) =>
        names is null
            ? Resources.SuitableToolkitNotFound
            : string.Format(
                Resources.SuitableToolkitXNotFound,
                LinguisticServices.CombineWithOr(names.Select(LinguisticServices.SingleQuote)));

    public static string ModuleNotFound(string? name) => string.Format(Resources.ModuleXNotFound, name);

    public static string CannotStartProcess(string? name) => string.Format(Resources.CannotStartProcessX, name);
}
