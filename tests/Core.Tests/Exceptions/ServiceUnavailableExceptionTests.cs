// <copyright file="ServiceUnavailableExceptionTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Livestock.Sdk.Core.Tests.Exceptions;

using Defra.Livestock.Sdk.Core.Exceptions;

/// <summary>
/// Unit tests for <see cref="ServiceUnavailableException"/>.
/// </summary>
public class ServiceUnavailableExceptionTests
{
    /// <summary>
    /// Verifies that <see cref="ServiceUnavailableException"/> can be initialized with a message.
    /// </summary>
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ServiceUnavailableException(message);

        // Assert
        exception.Message.ShouldBe(message);
    }

    /// <summary>
    /// Verifies that <see cref="ServiceUnavailableException"/> can be initialized with a message and inner exception.
    /// </summary>
    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsMessageAndInnerException()
    {
        // Arrange
        const string message = "Test message";

        var innerException = new InvalidOperationException("Inner exception message");

        // Act
        var exception = new ServiceUnavailableException(message, innerException);

        // Assert
        exception.Message.ShouldBe(message);
        exception.InnerException.ShouldBe(innerException);
    }
}
