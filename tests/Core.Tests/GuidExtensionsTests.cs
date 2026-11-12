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
    /// <param name="crockfordReplacement">Whether to replace ambiguous characters.</param>
    /// <param name="expected">The expected display hash.</param>
    [Theory]
    [InlineData(0, 3, true, "")]
    [InlineData(1, 3, true, "W")]
    [InlineData(6, 3, true, "WD7-QPP")]
    [InlineData(8, 3, true, "WD7-QPP-C3")]
    [InlineData(9, 1, true, "W-D-7-Q-P-P-C-3-9")]
    [InlineData(9, 9, true, "WD7QPPC39")]
    [InlineData(9, 12, true, "WD7QPPC39")]
    [InlineData(12, 4, true, "WD7Q-PPC3-9MTT")]
    [InlineData(12, 4, false, "WD7Q-PPC3-9MTT")]
    public void CreateDisplayHash_WithOptions_ReturnsExpectedHash(
        int length,
        int charactersPerGroup,
        bool crockfordReplacement,
        string expected)
    {
        var source = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");

        var result = source.CreateDisplayHash(length, charactersPerGroup, crockfordReplacement);

        result.ShouldBe(expected);
    }
}
