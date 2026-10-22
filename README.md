# lis-sdk-nuget-core

Part of the Livestock Information Service (LIS), which lets livestock keepers, owners, agents and
delegates record and manage their animals.

`Defra.Lis.Core` is a shared NuGet package that provides common building blocks for LIS .NET services: shared exception types, request header names, constants and string extensions.

Used by: LIS .NET services such as [lis-api-cattle](https://github.com/DEFRA/lis-api-cattle).

## Installation

```bash
dotnet add package Defra.Lis.Core
```

The package is published to [nuget.org](https://www.nuget.org/packages/Defra.Lis.Core) and targets `net10.0`.

## Usage

The package has no configuration and needs no dependency injection registration.

### Exceptions (`Defra.Lis.Core.Exceptions`)

Each exception takes a single message. Throw them from service code so that the calling service can map them to HTTP responses.

| Type | Use when | Sealed |
|---|---|---|
| `NotFoundException` | A requested record does not exist | No |
| `BusinessRuleException` | A business rule is broken | Yes |
| `ConflictException` | The request conflicts with the current state | Yes |
| `ProxyException` | A proxy error occurs | No |

```csharp
using Defra.Lis.Core.Exceptions;

if (holding is null)
{
    throw new NotFoundException($"Holding {cph} was not found");
}

if (animal.IsDead)
{
    throw new BusinessRuleException("A dead animal cannot be moved");
}

if (await repository.ExistsAsync(tag))
{
    throw new ConflictException($"Tag {tag} is already in use");
}

throw new ProxyException("The CDP proxy could not be configured");
```

### Request header names (`Defra.Lis.Core.Middleware.Headers.RequestHeaderNames`)

String constants for the headers LIS services pass between each other.

| Constant | Value |
|---|---|
| `CorrelationId` | `x-correlation-id` |
| `OperatorId` | `x-operator-id` |
| `ApiKey` | `x-api-key` |

```csharp
using Defra.Lis.Core.Middleware.Headers;

var correlationId = httpContext.Request.Headers[RequestHeaderNames.CorrelationId];
httpClient.DefaultRequestHeaders.Add(RequestHeaderNames.OperatorId, operatorId);
```

### Constants (`Defra.Lis.Core.Constants`)

| Constant | Value | Meaning |
|---|---|---|
| `ProxyName` | `CDP_HTTPS_PROXY` | Name of the environment variable that holds the CDP HTTPS proxy address |

```csharp
using Defra.Lis.Core;

var proxyAddress = Environment.GetEnvironmentVariable(Constants.ProxyName);
```

### String extensions (`Defra.Lis.Core.Extensions.StringExtensions`)

`ToSnakeCase(this string text)` converts a string to snake case. It lower-cases the first character and puts an underscore before each later upper-case character. It returns `null` or an empty string unchanged.

```csharp
using Defra.Lis.Core.Extensions;

"HoldingNumber".ToSnakeCase(); // "holding_number"
"cph".ToSnakeCase();           // "cph"
```

## Prerequisites

- .NET SDK 10 (the project targets `net10.0`)
- [Cake](https://cakebuild.net/) (`dotnet tool install -g Cake.Tool`), to run the build script locally
- To try a change in a consuming service: a local clone of [lis-dev](https://github.com/DEFRA/lis-dev), which runs a local NuGet server

## Setup

Inside a lis-dev checkout, restore fails with NU1507, because lis-dev's `nuget.config` lists two package sources without a package source mapping. Build from a standalone clone, or pass `--configfile` pointing at a config that lists only nuget.org.

### Development

```bash
dotnet restore Core.slnx
dotnet build Core.slnx
```

### Test

Tests use xUnit v3 with Shouldly for assertions and NSubstitute for mocking. There is nothing to install beyond the .NET SDK.

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

```bash
dotnet test Core.slnx
```

## Pipelines

Build, versioning and publishing run in GitHub Actions (`.github/workflows`) using Cake scripts (`*.cake`):

- **Push to a `feature/LREG-*` or `hotfix/LREG-*` branch** (`build.yml`): build, test, pack and Sonar analysis. Nothing is published.
- **Pull request, approval of a pull request, or push to `main`** (`pull-request.yml`, `pull-request-approved.yml`): work out the version, build, test, pack and run Sonar analysis. If the Sonar quality gate passes, the pipeline tags the commit, publishes to
  [nuget.org](https://www.nuget.org/packages/Defra.Lis.Core) and creates a GitHub release.

Versions come from [GitVersion](https://gitversion.net/) (`GitVersion.yml`, `version.cake`). Pull request branches
publish prereleases such as `0.2.0-lreg-41-alpha.3.27`. The ticket number in the version is lower case.

### Running the build locally

If you want to test the dotnet cake build locally you can run the following command:

```bash
dotnet cake ./build.cake --target=Default --product_name=DEFRA_lis_sdk_nuget_core --solution_file_name=Core.slnx --package_version=0.1.0
```

### Testing the GitHub Actions locally

Install `act` to run the GitHub Actions locally:

```bash
brew install act
```

Then run a workflow:

```bash
act -W .github/workflows/build.yml
```

## Branching policy

The repo uses mainline development. Branch from `main` and name the branch:

- `feature/LREG-<ticket>-<short-kebab-description>`
- `hotfix/LREG-<ticket>-<short-kebab-description>`

`LREG` must be uppercase, because the GitHub ruleset rejects other branch names. Rebase onto `main`
rather than merging `main` in.

## Contributing to this project

- Follow the [Defra software development standards](https://github.com/DEFRA/software-development-standards).
- Code style is enforced by StyleCop (`stylecop.json`) and `dotnet format`, which the build runs. Warnings are treated as errors.
- Open a pull request against `main`. CI must pass and the Sonar quality gate must be green before merge.

## Licence

THIS INFORMATION IS LICENSED UNDER THE CONDITIONS OF THE OPEN GOVERNMENT LICENCE found at:

<http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3>

The following attribution statement MUST be cited in your products and applications when using this information.

> Contains public sector information licensed under the Open Government licence v3

### About the licence

The Open Government Licence (OGL) was developed by the Controller of His Majesty's Stationery Office (HMSO) to enable information providers in the public sector to license the use and re-use of their information under a common open licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.
