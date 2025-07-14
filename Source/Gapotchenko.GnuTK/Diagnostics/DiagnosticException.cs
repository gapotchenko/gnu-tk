// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Diagnostics;

/// <summary>
/// Represents a diagnostic error that can occur in GNU-TK.
/// </summary>
sealed class DiagnosticException : ProgramException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramException"/> class with a specified error message
    /// and a diagnostic code.
    /// </summary>
    /// <inheritdoc cref="ProgramException(string?)"/>
    /// <param name="code">The diagnostic code.</param>
    public DiagnosticException(string? message, DiagnosticCode code) :
        base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticException"/> class with a specified error message,
    /// diagnostic code
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <inheritdoc cref="ProgramException(string?, Exception?)"/>
    /// <param name="code">The diagnostic code.</param>
    public DiagnosticException(string? message, DiagnosticCode code, Exception? innerException) :
        base(message, innerException)
    {
        Code = code;
    }

    /// <summary>
    /// Gets or initializes the diagnostic code.
    /// </summary>
    public DiagnosticCode Code { get; }
}
