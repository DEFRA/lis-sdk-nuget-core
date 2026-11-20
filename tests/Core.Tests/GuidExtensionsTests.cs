// <copyright file="GuidExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Livestock.Sdk.Core.Tests;

using Defra.Livestock.Sdk.Core.Extensions;

/// <summary>
/// Unit tests for <see cref="GuidExtensions"/>.
/// </summary>
public class GuidExtensionsTests
{
    /// <summary>
    /// Verifies stable display hashes for known GUIDs.
    /// </summary>
    /// <param name="input">The GUID.</param>
    /// <param name="expected">The expected display hash.</param>
    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", "2AW-KEZ-5YF")]
    [InlineData("00112233-4455-6677-8899-aabbccddeeff", "WD7-QPP-C39")]
    public void CreateDisplayHash_WithDefaults_ReturnsExpectedHash(string input, string expected)
    {
        var result = Guid.Parse(input).CreateDisplayHash();

        result.ShouldBe(expected);
    }

    /// <summary>
    /// Verifies custom lengths and grouping without trailing separators.
    /// </summary>
    /// <param name="length">The number of hash characters.</param>
    /// <param name="charactersPerGroup">The group size.</param>
    /// <param name="expected">The expected display hash.</param>
    [Theory]
    [InlineData(0, 3, "")]
    [InlineData(1, 3, "W")]
    [InlineData(6, 3, "WD7-QPP")]
    [InlineData(8, 3, "WD7-QPP-C3")]
    [InlineData(9, 1, "W-D-7-Q-P-P-C-3-9")]
    [InlineData(9, 9, "WD7QPPC39")]
    [InlineData(9, 12, "WD7QPPC39")]
    [InlineData(12, 4, "WD7Q-PPC3-9MTT")]
    public void CreateDisplayHash_WithOptions_ReturnsExpectedHash(
        int length,
        int charactersPerGroup,
        string expected)
    {
        var source = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");
        var result = source.CreateDisplayHash(length, charactersPerGroup);
        result.ShouldBe(expected);
    }

    /// <summary>
    /// Verifies out-of-range options are rejected.
    /// </summary>
    /// <param name="length">The number of hash characters.</param>
    /// <param name="charactersPerGroup">The group size.</param>
    /// <param name="paramName">The parameter expected to be reported.</param>
    [Theory]
    [InlineData(-1, 3, "length")]
    [InlineData(13, 3, "length")]
    [InlineData(9, 0, "charactersPerGroup")]
    [InlineData(9, -1, "charactersPerGroup")]
    public void CreateDisplayHash_WithInvalidOptions_ThrowsArgumentOutOfRangeException(
        int length,
        int charactersPerGroup,
        string paramName)
    {
        var source = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");

        var exception = Should.Throw<ArgumentOutOfRangeException>(
            () => source.CreateDisplayHash(length, charactersPerGroup));

        exception.ParamName.ShouldBe(paramName);
    }
}
