// <copyright file="ServiceUnavailableException.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Livestock.Sdk.Core.Exceptions;

/// <summary>
/// Exception thrown when a service is unavailable.
/// </summary>
public class ServiceUnavailableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ServiceUnavailableException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The inner exception.</param>
    public ServiceUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
