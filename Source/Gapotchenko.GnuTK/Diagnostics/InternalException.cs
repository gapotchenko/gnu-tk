// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK.Diagnostics;

/// <summary>
/// Represents an internal error that can occur in GNU-TK.
/// </summary>
sealed class InternalException : ProductException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalException"/> class.
    /// </summary>
    public InternalException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalException"/> class with a specified error message.
    /// </summary>
    /// <inheritdoc/>
    public InternalException(string? message) :
        base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <inheritdoc/>
    public InternalException(string? message, Exception? innerException) :
        base(message, innerException)
    {
    }
}
