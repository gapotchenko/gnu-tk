// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK;

/// <summary>
/// Represents an error that can occur in GNU-TK.
/// </summary>
class GnuTKException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GnuTKException"/> class.
    /// </summary>
    public GnuTKException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GnuTKException"/> class with a specified error message.
    /// </summary>
    /// <inheritdoc/>
    public GnuTKException(string? message) :
        base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GnuTKException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <inheritdoc/>
    public GnuTKException(string? message, Exception? innerException) :
        base(message, innerException)
    {
    }
}
