// Gapotchenko.GnuTK
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.GnuTK;

/// <summary>
/// Represents a classified error that can occur in GNU-TK.
/// </summary>
sealed class GnuTKClassifiedException : GnuTKException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GnuTKClassifiedException"/> class.
    /// </summary>
    public GnuTKClassifiedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GnuTKClassifiedException"/> class with a specified error message.
    /// </summary>
    /// <inheritdoc/>
    public GnuTKClassifiedException(string? message) :
        base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GnuTKClassifiedException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <inheritdoc/>
    public GnuTKClassifiedException(string? message, Exception? innerException) :
        base(message, innerException)
    {
    }

    /// <summary>
    /// Gets or initializes the error code.
    /// </summary>
    public required int ErrorCode { get; init; }
}
