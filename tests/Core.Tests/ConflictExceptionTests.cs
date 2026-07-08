// <copyright file="ConflictExceptionTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Core.Tests;

using Defra.Core.Exceptions;

/// <summary>
/// Unit tests for <see cref="ConflictException"/>.
/// </summary>
public class ConflictExceptionTests
{
    /// <summary>
    /// Verifies that <see cref="ConflictException"/> can be initialized with a message.
    /// </summary>
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Test message";

        // Act
        var exception = new ConflictException(message);

        // Assert
        exception.Message.ShouldBe(message);
    }
}
