// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Diagnostics;

static class DiagnosticServices
{
    public static string GetErrorIdentifier(DiagnosticCode? errorCode)
    {
        return $"GNUTK{(int?)errorCode:D4}";
    }
}
