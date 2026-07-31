// <copyright file="RequestHeaderNames.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Lis.Core.Middleware.Headers;

/// <summary>
/// Request header names.
/// </summary>
public static class RequestHeaderNames
{
    /// <summary>
    /// Correlation ID header name.
    /// </summary>
    public const string CorrelationId = "x-correlation-id";

    /// <summary>
    /// Operator ID header name.
    /// </summary>
    public const string OperatorId = "x-operator-id";

    /// <summary>
    /// Api key header name.
    /// </summary>
    public const string ApiKey = "x-api-key";
}
