// <copyright file="ConflictException.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Core.Exceptions;

using System;

/// <summary>
/// Exception thrown when a conflict occurs.
/// </summary>
public sealed class ConflictException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ConflictException(string message)
        : base(message)
    {
    }
}
