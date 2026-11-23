// <copyright file="StringExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Livestock.Sdk.Core.Tests.Extensions;

using Defra.Livestock.Sdk.Core.Extensions;

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
        var result = input.ToSnakeCase();

        // Assert
        result.ShouldBe(expected);
    }

    /// <summary>
    /// Verifies GUID strings are normalized before hashing.
    /// </summary>
    /// <param name="input">The GUID string.</param>
    [Theory]
    [InlineData("00112233-4455-6677-8899-aabbccddeeff")]
    [InlineData("00112233-4455-6677-8899-AABBCCDDEEFF")]
    [InlineData("00112233445566778899aabbccddeeff")]
    [InlineData("{00112233-4455-6677-8899-aabbccddeeff}")]
    [InlineData("(00112233-4455-6677-8899-aabbccddeeff)")]
    [InlineData(" 00112233-4455-6677-8899-aabbccddeeff ")]
    public void CreateDisplayHash_WithValidGuid_ReturnsExpectedHash(string input)
    {
        input.CreateDisplayHash().ShouldBe("WD7-QPP-C39");
    }

    /// <summary>
    /// Verifies hashing options are forwarded to the GUID extension.
    /// </summary>
    [Fact]
    public void CreateDisplayHash_WithOptions_ReturnsExpectedHash()
    {
        var result = "00112233-4455-6677-8899-aabbccddeeff"
            .CreateDisplayHash(12, 4);

        result.ShouldBe("WD7Q-PPC3-9MTT");
    }

    /// <summary>
    /// Verifies a null string is rejected.
    /// </summary>
    [Fact]
    public void CreateDisplayHash_WithNull_ThrowsArgumentNullException()
    {
        string? input = null;

        Should.Throw<ArgumentNullException>(() => input.CreateDisplayHash());
    }

    /// <summary>
    /// Verifies invalid GUID strings produce a meaningful exception.
    /// </summary>
    /// <param name="input">The invalid GUID string.</param>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-guid")]
    [InlineData("00112233-4455-6677-8899-aabbccddeefg")]
    public void CreateDisplayHash_WithInvalidGuid_ThrowsFormatException(string input)
    {
        var exception = Should.Throw<FormatException>(() => input.CreateDisplayHash());

        exception.Message.ShouldBe("Invalid GUID");
    }
}
