// <copyright file="NotFoundExceptionTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace DefraDigital.Lis.Core.Tests;

using Defra.Lis.Core.Exceptions;

/// <summary>
/// Unit tests for <see cref="NotFoundException"/>.
/// </summary>
public class NotFoundExceptionTests
{
    /// <summary>
    /// Verifies that <see cref="NotFoundException"/> can be initialized with a message.
    /// </summary>
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Test message";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Message.ShouldBe(message);
    }
}
