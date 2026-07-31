// <copyright file="StringExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Lis.Core.Tests;

using Defra.Lis.Core.Extensions;

/// <summary>
/// Unit tests for <see cref="StringExtensions"/>.
/// </summary>
public class StringExtensionsTests
{
    /// <summary>
    /// Verifies that <see cref="StringExtensions.ToSnakeCase"/> converts camel case to snake case.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="expected">The expected snake case string.</param>
    [Theory]
    [InlineData("CamelCase", "camel_case")]
    [InlineData("camelCase", "camel_case")]
    [InlineData("String", "string")]
    [InlineData("snake_case", "snake_case")]
    [InlineData("ABC", "a_b_c")]
    [InlineData("a", "a")]
    [InlineData("A", "a")]
    [InlineData("My123String", "my123_string")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void ToSnakeCase_ReturnsExpectedResult(string? input, string? expected)
    {
        // Act
        var result = input!.ToSnakeCase();

        // Assert
        result.ShouldBe(expected);
    }
}
