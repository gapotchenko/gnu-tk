namespace Gapotchenko.GnuTK.Diagnostics;

static class DiagnosticServices
{
    public static string GetErrorIdentifier(DiagnosticCode? errorCode)
    {
        return $"GNUTK{(int?)errorCode:D4}";
    }
}
