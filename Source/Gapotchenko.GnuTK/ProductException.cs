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
class ProductException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductException"/> class.
    /// </summary>
    public ProductException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductException"/> class with a specified error message.
    /// </summary>
    /// <inheritdoc/>
    public ProductException(string? message) :
        base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <inheritdoc/>
    public ProductException(string? message, Exception? innerException) :
        base(message, innerException)
    {
    }
}
