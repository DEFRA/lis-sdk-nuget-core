// <copyright file="NotFoundException.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>
namespace Defra.Lis.Core.Exceptions;

/// <summary>
/// Exception thrown when a Not found error occurs.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }
}
