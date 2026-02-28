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
enum DiagnosticCode
{
    MissingProgramArguments = 1,
    InvalidProgramArguments = 2,
    ConflictingProgramArguments = 3,
    SuitableToolkitNotFound = 4,
    ConfigurationFileLoadError = 5,
    BuiltinToolkitDirectoryNotFound = 6
}
