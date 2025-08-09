// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Diagnostics;

/// <summary>
/// Defines the diagnostic codes for GNU-TK.
/// </summary>
/// <remarks>
/// Must be in sync with <c>Documentation/Diagnostics/Errors.txt</c> file.
/// </remarks>
public enum DiagnosticCode
{
    InvalidProgramArguments = 1,
    SuitableToolkitNotFound = 2,
    ConflictingProgramArguments = 3,
    ConfigurationFileLoadError = 4
}
