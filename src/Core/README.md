# Defra.Livestock.Sdk.Core

Shared building blocks for Livestock Information Service (LIS) .NET services. Targets `net10.0`; no configuration or dependency injection registration is required.

## Installation

```sh
dotnet add package Defra.Livestock.Sdk.Core
```

This is the package ID produced by the current project.

## Contents

| API | Namespace | Purpose |
|---|---|---|
| `Constants.ProxyName` | `Defra.Livestock.Sdk.Core` | CDP proxy environment variable name: `CDP_HTTPS_PROXY` |
| `BusinessRuleException` | `Defra.Livestock.Sdk.Core.Exceptions` | A business rule is violated |
| `ConflictException` | `Defra.Livestock.Sdk.Core.Exceptions` | A request conflicts with the current state |
| `NotFoundException` | `Defra.Livestock.Sdk.Core.Exceptions` | A requested record does not exist |
| `ProxyException` | `Defra.Livestock.Sdk.Core.Exceptions` | A proxy error occurs |
| `RequestHeaderNames` | `Defra.Livestock.Sdk.Core.Middleware.Headers` | `CorrelationId`, `OperatorId` and `ApiKey` header constants |
| `StringExtensions.ToSnakeCase()` | `Defra.Livestock.Sdk.Core.Extensions` | Convert strings to snake case |
| `GuidExtensions.CreateDisplayHash()` | `Defra.Livestock.Sdk.Core.Extensions` | Generate a deterministic display hash from a GUID |
| `StringExtensions.CreateDisplayHash()` | `Defra.Livestock.Sdk.Core.Extensions` | Parse a GUID string and generate its display hash |

Each exception accepts a message. Request header values are `x-correlation-id`, `x-operator-id` and `x-api-key` respectively.

## Usage

```csharp
using System;
using Defra.Livestock.Sdk.Core.Extensions;
using Defra.Livestock.Sdk.Core;
using Defra.Livestock.Sdk.Core.Exceptions;
using Defra.Livestock.Sdk.Core.Middleware.Headers;

var proxyAddress = Environment.GetEnvironmentVariable(Constants.ProxyName);
var correlationHeader = RequestHeaderNames.CorrelationId;
var fieldName = "HoldingNumber".ToSnakeCase(); // "holding_number"
var id = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");
var displayId = id.CreateDisplayHash();       // "WD7-QPP-C39"
var longerId = id.CreateDisplayHash(12, 4);    // "WD7Q-PPC3-9MTT"
var fromString = id.ToString().CreateDisplayHash(); // "WD7-QPP-C39"

throw new NotFoundException("Holding was not found");
```

`ToSnakeCase()` returns null and empty input unchanged and inserts an underscore before each upper-case character after the first (`ABC` becomes `a_b_c`).

Both display-hash overloads accept `length` (default `9`), `charactersPerGroup` (default `3`) and `crockfordReplacement` (default `true`). Length excludes separators. Use a positive group size and lengths from 0 to 12. Zero length returns an empty string. Hashes use SHA-256 and the Crockford Base32 alphabet, which already excludes the ambiguous letters handled by the replacement option.

The string overload accepts GUID formats supported by `Guid.TryParse`. Invalid input, including null and empty strings, throws `InvalidCastException("Invalid GUID")`.

Display hashes can collide; retain the original GUID for identity and uniqueness checks.

## Source

See the [source repository](https://github.com/defra/lis-sdk-nuget-core) for development, testing and publishing instructions. Building requires a .NET 10 SDK with C# 14 support.
