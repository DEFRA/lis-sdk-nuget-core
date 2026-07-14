// <copyright file="ProxyException.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Lis.Core.Exceptions;

/// <summary>
/// Exception thrown when a proxy error occurs.
/// </summary>
public class ProxyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ProxyException(string message)
        : base(message)
    {
    }
}
