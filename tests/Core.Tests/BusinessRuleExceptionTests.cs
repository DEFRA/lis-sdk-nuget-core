// <copyright file="BusinessRuleExceptionTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Core.Tests;

using Defra.Core.Exceptions;

/// <summary>
/// Unit tests for <see cref="BusinessRuleException"/>.
/// </summary>
public class BusinessRuleExceptionTests
{
    /// <summary>
    /// Verifies that <see cref="BusinessRuleException"/> can be initialized with a message.
    /// </summary>
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        var message = "Test message";

        // Act
        var exception = new BusinessRuleException(message);

        // Assert
        exception.Message.ShouldBe(message);
    }
}
