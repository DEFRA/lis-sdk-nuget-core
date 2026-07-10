
var productName = Argument<string>("product_name", "");
var solution_file_name = Argument<string>("solution_file_name", "");
var version = Argument<string>("package_version", "");
var sonarToken = Argument<string>("sonar_token", "");
const string SonarHostUrl = "https://sonarcloud.io";
const string SonarOrganization = "defra";
const string SonarCoverageFile = "coverage.xml";

Task("Sonar-Install")
    .Description("Installs the SonarCloud scanner and dotnet-coverage tools")
    .Does(() => {
        EnsureDirectoryExists("./.sonar/scanner");
        EnsureDirectoryExists("./.sonar/coverage");

        StartProcess("dotnet", new ProcessSettings {
            Arguments = "tool update dotnet-sonarscanner --tool-path ./.sonar/scanner"
        });

        StartProcess("dotnet", new ProcessSettings {
            Arguments = "tool update dotnet-coverage --tool-path ./.sonar/coverage"
        });
    });

Task("Sonar-Begin")
    .IsDependentOn("Sonar-Install")
    .IsDependentOn("Version")
    .Description("Starts SonarCloud analysis")
    .Does(() => {
        if (string.IsNullOrWhiteSpace(sonarToken))
        {
            throw new Exception("SONAR_TOKEN environment variable is required to run SonarCloud analysis.");
        }

        StartProcess(SonarScannerPath, new ProcessSettings {
            Arguments = string.Join(" ", new [] {
                "begin",
                $"/k:\"{productName}\"",
                "/o:\"defra\"",
                $"/d:sonar.token=\"{sonarToken}\"",
                "/d:sonar.host.url=\"https://sonarcloud.io\"",
                $"/v:\"{version}\"",
                "/d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml",
                "/d:sonar.exclusions=\"changelog/**,.github/**\"",
                "/d:sonar.dotnet.excludeTestProjects=true"
            })
        });
    });

Task("Sonar-Build")
    .IsDependentOn("Sonar-Begin")
    .Description("Builds the solution for SonarCloud analysis")
    .Does(() => {
        DotNetBuild(solution_file_name, new DotNetBuildSettings {
            Configuration = configuration,
            NoIncremental = true,
            ArgumentCustomization = args => args.Append($"/p:Version={version}")
        });
    });

Task("Sonar-Test")
    .IsDependentOn("Sonar-Build")
    .Description("Runs tests and collects coverage for SonarCloud")
    .Does(() => {
        StartProcess(DotNetCoveragePath, new ProcessSettings {
            Arguments = $"collect \"dotnet test --configuration {configuration} --no-build\" -f xml -o \"{SonarCoverageFile}\""
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

        StartProcess(SonarScannerPath, new ProcessSettings {
            Arguments = $"end /d:sonar.token=\"{sonarToken}\""
        });
    });

Task("Sonar")
    .IsDependentOn("Sonar-End")
    .Description("Runs the full SonarCloud analysis pipeline");

RunTarget(target);