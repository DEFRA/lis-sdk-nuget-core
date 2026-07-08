#nullable enable
#addin nuget:?package=Cake.Sonar&version=5.0.0
#tool dotnet:?package=dotnet-sonarscanner&version=11.2.1

#load "base.cake"

using System;
using System.Collections.Generic;
using System.Linq;

var target = Argument("target", "Default");
var productName = Argument("product_name", "");
var configuration = Argument("configuration", "Release");
var repositoryRoot = MakeAbsolute(Directory("../"));
var version = string.Empty;
var branchName = string.Empty;
var sonarToken = EnvironmentVariable("SONAR_TOKEN");
const string SonarHostUrl = "https://sonarcloud.io";
const string SonarOrganization = "defra";
const string SonarCoverageFile = "coverage.xml";

IReadOnlyList<FilePath> GetBuildTargets()
{
    var solutions = GetFiles($"{repositoryRoot}/*.slnx").Concat(GetFiles($"{repositoryRoot}/*.sln")).ToList();
    if (solutions.Any())
    {
        return solutions;
    }

    return GetFiles($"{repositoryRoot}/**/*.csproj").ToList();
}

IReadOnlyList<FilePath> GetTestTargets()
{
    var solutions = GetFiles($"{repositoryRoot}/*.slnx").Concat(GetFiles("./*.sln")).ToList();
    if (solutions.Any())
    {
        return solutions;
    }

    var testProjects = GetFiles($"{repositoryRoot}/tests/**/*.csproj")
            .Concat(GetFiles($"{repositoryRoot}/**/*Tests.csproj"))
            .Concat(GetFiles($"{repositoryRoot}/**/*.Tests.csproj"))
            .Distinct()
            .ToList();

    return testProjects;
}

var buildTargets = GetBuildTargets();
var testTargets = GetTestTargets();

if (!buildTargets.Any())
{
    throw new Exception("No .NET solution or project files were found for dotnet-build-test.cake.");
}

Task("FetchVersionState")
    .Does(() =>
    {
        version = CalculateVersion();
        
    });

Task("CalculateVersion")
    .IsDependentOn("FetchVersionState")
    .Does(() =>
    {
        branchName = ResolveRepositoryBranchName(Argument("branch", string.Empty));
        version = CalculateRepositoryVersion(Argument("branch", string.Empty)).Version;
        Environment.SetEnvironmentVariable("BUILD_VERSION", version);
        Information($"Using build version '{version}'.");
    });

Task("CoverageTool")
    .IsDependentOn("CalculateVersion")
    .Description("Installs the dotnet-coverage tool")
    .Does(() => {
        EnsureDirectoryExists("./.sonar/coverage");
        StartProcess("dotnet", new ProcessSettings {
            Arguments = "tool update dotnet-coverage --tool-path ./.sonar/coverage"
        });
    });

Task("Sonar-Begin")
    .IsDependentOn("CoverageTool")
    .Description("Starts SonarCloud analysis")
    .Does(() => {
        if (string.IsNullOrWhiteSpace(sonarToken))
        {
            throw new Exception("SONAR_TOKEN environment variable is required to run SonarCloud analysis.");
        }

        SonarBegin(new SonarBeginSettings
        {
            Key = productName,
            Organization = SonarOrganization,
            Url = SonarHostUrl,
            Token = sonarToken,
            Version = version,
            Branch = branchName,
            Exclusions = "changelog/**,.github/**",
            VsCoverageReportsPath = SonarCoverageFile,
            UseCoreClr = true,
            Verbose = true
        });
    });

Task("Sonar-Build")
    .IsDependentOn("Sonar-Begin")
    .Description("Builds the solution for SonarCloud analysis")
    .Does(() => {
        var settings = new DotNetBuildSettings
        {
            Configuration = configuration,
            NoIncremental = true,
            ArgumentCustomization = args => args.Append($"/p:Version={version}")
        };

        foreach (var buildTarget in buildTargets)
        {
            Information($"Building {buildTarget} for Sonar analysis.");
            DotNetBuild(buildTarget.FullPath, settings);
        }
    });

Task("Sonar-Test")
    .IsDependentOn("Sonar-Build")
    .Description("Runs tests and collects coverage for SonarCloud")
    .Does(() => {
        if (!testTargets.Any())
        {
            Information("No .NET test projects were found. Skipping test execution for Sonar.");
            return;
        }

        var testCommand = buildTargets.Count == 1
            ? $"dotnet test \"{buildTargets[0].FullPath}\" --configuration {configuration} --no-build"
            : $"dotnet test --configuration {configuration} --no-build";

        StartProcess("./.sonar/coverage/dotnet-coverage", new ProcessSettings {
            Arguments = $"collect \"{testCommand}\" -f xml -o \"{SonarCoverageFile}\""
        });
    });

Task("Sonar-End")
    .IsDependentOn("Sonar-Test")
    .Description("Completes SonarCloud analysis")
    .Does(() => {
        if (string.IsNullOrWhiteSpace(sonarToken))
        {
            throw new Exception("SONAR_TOKEN environment variable is required to run SonarCloud analysis.");
        }

        SonarEnd(new SonarEndSettings
        {
            Token = sonarToken,
            UseCoreClr = true
        });
    });

Task("DockerBuild")
    .IsDependentOn("Sonar-End")
    .WithCriteria(() => FileExists("./Dockerfile"))
    .Does(() =>
    {
        Information($"Dockerfile found. Building container image for version '{version}'.");
        RunCommand("docker", $"build --no-cache --tag {version} .");
    });

Task("Default")
    .IsDependentOn("DockerBuild");

RunTarget(target);
