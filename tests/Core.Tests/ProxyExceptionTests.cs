// <copyright file="ProxyExceptionTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace DefraDigital.Lis.Core.Tests;

using DefraDigital.Lis.Core.Exceptions;

/// <summary>
/// Unit tests for <see cref="ProxyException"/>.
/// </summary>
public class ProxyExceptionTests
{
    /// <summary>
    /// Verifies that <see cref="ProxyException"/> can be initialized with a message.
    /// </summary>
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Test message";

        // Act
        var exception = new ProxyException(message);

        // Assert
        exception.Message.ShouldBe(message);
    }
}
