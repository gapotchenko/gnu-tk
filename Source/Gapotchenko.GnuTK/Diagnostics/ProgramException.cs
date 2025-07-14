// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Diagnostics;

/// <summary>
/// Represents an error that can occur in GNU-TK program.
/// </summary>
class ProgramException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramException"/> class.
    /// </summary>
    public ProgramException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramException"/> class with a specified error message.
    /// </summary>
    /// <inheritdoc/>
    public ProgramException(string? message) :
        base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <inheritdoc/>
    public ProgramException(string? message, Exception? innerException) :
        base(message, innerException)
    {
    }
}
