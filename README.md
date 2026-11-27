# lis-sdk-nuget-core

Part of the Livestock Information Service (LIS), which lets livestock keepers, owners, agents and
delegates record and manage their animals.

`Defra.Livestock.Sdk.Core` is a shared NuGet package that provides common building blocks for LIS .NET services: shared exception types, request header names, constants, string extensions and GUID display hashes.

Used by: LIS .NET services such as [lis-api-cattle](https://github.com/DEFRA/lis-api-cattle).

## Installation

```bash
dotnet add package Defra.Livestock.Sdk.Core
```

The project produces the `Defra.Livestock.Sdk.Core` package and targets `net10.0`. Publishing is configured in the [tag-triggered workflow](https://github.com/DEFRA/lis-sdk-nuget-core/blob/main/.github/workflows/publish.yml) and [composite NuGet action](https://github.com/DEFRA/lis-github-actions/blob/5f5557f64c1344efb6ed44e482452f9cfe6bea51/actions/publish-nuget/action.yml).

## Usage

The package has no configuration and needs no dependency injection registration.

### Exceptions (`Defra.Livestock.Sdk.Core.Exceptions`)

Each exception takes a message; `ServiceUnavailableException` can also take an inner exception. Throw them from service code so that the calling service can map them to HTTP responses.

| Type | Use when | Sealed |
|---|---|---|
| `NotFoundException` | A requested record does not exist | No |
| `ConflictException` | The request conflicts with the current state | Yes |
| `ProxyException` | A proxy error occurs | No |
| `ServiceUnavailableException` | A downstream service is unavailable | No |

```csharp
using Defra.Livestock.Sdk.Core.Exceptions;

if (holding is null)
{
    throw new NotFoundException($"Holding {cph} was not found");
}

if (await repository.ExistsAsync(tag))
{
    throw new ConflictException($"Tag {tag} is already in use");
}

throw new ProxyException("The CDP proxy could not be configured");
```

```csharp
try
{
    response = await httpClient.GetAsync(uri);
}
catch (HttpRequestException ex)
{
    throw new ServiceUnavailableException("The cattle API is unavailable", ex);
}
```

### Request header names (`Defra.Livestock.Sdk.Core.Middleware.Headers.RequestHeaderNames`)

String constants for the headers LIS services pass between each other.

| Constant | Value |
|---|---|
| `CorrelationId` | `x-correlation-id` |
| `OperatorId` | `x-operator-id` |
| `ApiKey` | `x-api-key` |
| `CdpRequestId` | `x-cdp-request-id` |

```csharp
using Defra.Livestock.Sdk.Core.Middleware.Headers;

var correlationId = httpContext.Request.Headers[RequestHeaderNames.CorrelationId];
httpClient.DefaultRequestHeaders.Add(RequestHeaderNames.OperatorId, operatorId);
```

### Constants (`Defra.Livestock.Sdk.Core.Constants`)

| Constant | Value | Meaning |
|---|---|---|
| `ProxyName` | `CDP_HTTPS_PROXY` | Name of the environment variable that holds the CDP HTTPS proxy address |

```csharp
using Defra.Livestock.Sdk.Core;

var proxyAddress = Environment.GetEnvironmentVariable(Constants.ProxyName);
```

### String extensions (`Defra.Livestock.Sdk.Core.Extensions.StringExtensions`)

Import `Defra.Livestock.Sdk.Core.Extensions` to use the string and GUID extension methods.

`ToSnakeCase()` lower-cases the first character and inserts an underscore before each later upper-case character. It returns `null` or an empty string unchanged. Consecutive capitals are separated individually.

```csharp
using System;
using Defra.Livestock.Sdk.Core.Extensions;

"HoldingNumber".ToSnakeCase(); // "holding_number"
"cph".ToSnakeCase();           // "cph"
"ABC".ToSnakeCase();           // "a_b_c"
```

### Display hashes (`Defra.Livestock.Sdk.Core.Extensions.GuidExtensions` and `Defra.Livestock.Sdk.Core.Extensions.StringExtensions`)

`CreateDisplayHash(int length = 9, int charactersPerGroup = 3)` produces a deterministic, upper-case display identifier from a GUID. The string overload parses a GUID first, so casing and GUID formatting do not change the result. A `null` string throws `ArgumentNullException`; any other invalid GUID string, including an empty string, throws `FormatException` with the message `Invalid GUID`.

```csharp
using System;
using Defra.Livestock.Sdk.Core.Extensions;

var id = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");
id.CreateDisplayHash();                  // "WD7-QPP-C39"
id.CreateDisplayHash(12, 4);             // "WD7Q-PPC3-9MTT"
id.CreateDisplayHash(9, 9);              // "WD7QPPC39"
id.ToString().CreateDisplayHash();       // "WD7-QPP-C39"
```

`length` counts hash characters, excluding hyphen separators. `length` must be from 0 to 12 and `charactersPerGroup` must be positive; other values throw `ArgumentOutOfRangeException`. A length of zero produces an empty string. The implementation reads 64 bits from a SHA-256 digest and encodes five bits per character using the Crockford Base32 alphabet. The alphabet excludes `I`, `L`, `O` and `U`, so hashes never contain characters that are easily confused.

Display hashes can collide. Use the original GUID for identity and uniqueness checks.

## Prerequisites

- .NET 10 SDK with C# 14 support (the project targets `net10.0` and uses extension blocks). SDK selection and the Microsoft Testing Platform runner are configured in [global.json](https://github.com/DEFRA/lis-sdk-nuget-core/blob/main/global.json).
- To try a change in a consuming service: a local clone of [lis-dev](https://github.com/DEFRA/lis-dev), which runs a local NuGet server

## Setup

### Development

```bash
dotnet restore Core.slnx
dotnet build Core.slnx
```

## Running in development

This is a library, so it has no service of its own. To try a change in a consuming service before
publishing, pack it and push it to the local NuGet server that lis-dev runs on port 5555
(`nuget-server` in its compose stack, already listed in lis-dev's `nuget.config`):

```bash
dotnet pack src/Core/Core.csproj -c Release -o ./artifacts /p:Version=0.0.0-local.1
dotnet nuget push "./artifacts/*.nupkg" --source http://localhost:5555/v3/index.json --api-key LOCAL-NUGET-API-KEY
```

Then reference that version from the consuming service.

## Running tests

Tests use xUnit v3 with Shouldly assertions and Microsoft Testing Platform. Coverage includes exception messages, constants, snake-case conversion and GUID display hashing.

Run the xUnit test executable directly after building:

```bash
dotnet build tests/Core.Tests/Core.Tests.csproj
dotnet tests/Core.Tests/bin/Debug/net10.0/Defra.Livestock.Sdk.Core.Tests.dll
```

## Pipelines

GitHub Actions use the composite actions under `actions/` in `DEFRA/lis-github-actions`. Every workflow pins these actions to the `v0.11.0` release commit (`5f5557f64c1344efb6ed44e482452f9cfe6bea51`). Update all `uses:` references together when moving to a newer release.

| Workflow | Trigger | Configured behaviour |
|---|---|---|
| [Prerelease](https://github.com/DEFRA/lis-sdk-nuget-core/blob/main/.github/workflows/prerelease.yml) | Push to `feature/**` or `hotfix/**` | Calculate a prerelease version, validate and run Sonar analysis, then tag the commit when validation succeeds and the quality gate passes |
| [Pull Request Validation](https://github.com/DEFRA/lis-sdk-nuget-core/blob/main/.github/workflows/pull-request.yml) | Pull request opened, updated or reopened against `main` | Calculate a prospective version, validate and run Sonar analysis; aggregate the job results into a required check |
| [Release](https://github.com/DEFRA/lis-sdk-nuget-core/blob/main/.github/workflows/release.yml) | Pull request merged into `main` | Calculate a stable version, validate and run Sonar analysis, then tag the merged commit when validation succeeds and the quality gate passes |
| [Publish](https://github.com/DEFRA/lis-sdk-nuget-core/blob/main/.github/workflows/publish.yml) | Push of a tag matching `*.*.*` | Run the central composite NuGet publishing action in a local job |

NuGet's trusted publishing policy must use repository owner `DEFRA`, repository `lis-sdk-nuget-core`, workflow filename `publish.yml`, and environment `release`. The `NUGET_USERNAME` secret identifies the policy creator. The publishing job stays in this repository because NuGet rejects authentication inside a reusable workflow hosted in another repository.

Publishing requires an artifact built from the tagged commit by a successful prerelease or release workflow. The artifact records its commit, version, workflow run and attempt; publishing waits for that attempt to finish successfully before authenticating with NuGet.

Dependabot branches receive patch versions. Their PR validation versions use the label `dependabot-alpha.<run>.<attempt>`; feature and hotfix prereleases require an LREG ticket.

## Branching policy

The repo uses mainline development. Branch from `main` and name the branch:

- `feature/LREG-<ticket>-<short-kebab-description>`
- `hotfix/LREG-<ticket>-<short-kebab-description>`

`LREG` must be uppercase, because the GitHub ruleset rejects other branch names. Rebase onto `main`
rather than merging `main` in.

## Contributing to this project

- Follow the [Defra software development standards](https://github.com/DEFRA/software-development-standards).
- Code style is configured in `.editorconfig` and `stylecop.json`, with StyleCop and .NET analyzers enabled during builds. Warnings are treated as errors. Run `dotnet format Core.slnx --verify-no-changes` to check formatting locally.
- Open a pull request against `main`. CI must pass and the Sonar quality gate must be green before merge.

## Licence

THIS INFORMATION IS LICENSED UNDER THE CONDITIONS OF THE OPEN GOVERNMENT LICENCE found at:

<http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3>

The following attribution statement MUST be cited in your products and applications when using this information.

> Contains public sector information licensed under the Open Government licence v3

### About the licence

The Open Government Licence (OGL) was developed by the Controller of His Majesty's Stationery Office (HMSO) to enable information providers in the public sector to license the use and re-use of their information under a common open licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.

## Maintaining code analysis and public APIs

The build runs the .NET 10 recommended analyzer rules, StyleCop and SonarAnalyzer. Tests also receive xUnit analyzers through `xunit.v3`. Analyzer warnings are treated as errors; CA1707 is disabled for test code to allow descriptive names with underscores.

The library uses `Microsoft.CodeAnalysis.PublicApiAnalyzers` to track its public surface, including nullability and default parameter values. These files make API changes visible during review and help catch accidental changes to the contract used by consuming services.

Every merged PR to `main` enters the release pipeline, so there is no separate release preparation step. Update `src/Core/PublicAPI.Shipped.txt` in the same PR as the API change: it records the API contract that will be released on merge. Include both new types and their public members; for example, a new exception needs entries for both the type and its public constructor. The RS0016 code fix can generate declarations in `src/Core/PublicAPI.Unshipped.txt`; move those additions into the shipped file before merging.

Keep `PublicAPI.Unshipped.txt` empty apart from `#nullable enable` unless it contains intentional `*REMOVED*` entries. Retain `#nullable enable` at the top of both files. Preserve previously shipped declarations as the compatibility record, and represent intentional removals using `*REMOVED*` entries in the unshipped file. Review breaking changes explicitly, since consuming services can still depend on earlier package versions.
