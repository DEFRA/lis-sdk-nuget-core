// <copyright file="ConstantsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace DefraDigital.Lis.Core.Tests;

using Defra.Lis.Core;

/// <summary>
/// Unit tests for <see cref="Constants"/>.
/// </summary>
public class ConstantsTests
{
    /// <summary>
    /// Verifies that <see cref="Constants.ProxyName"/> has the correct value.
    /// </summary>
    [Fact]
    public void ProxyName_ShouldHaveCorrectValue()
    {
        // Assert
        Constants.ProxyName.ShouldBe("CDP_HTTPS_PROXY");
    }
}
