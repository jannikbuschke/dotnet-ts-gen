using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;
using static Nuke.Common.Tooling.ProcessTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Git.GitTasks;

// ReSharper disable VariableHidesOuterVariable
// ReSharper disable AllUnderscoreLocalParameterName
// ReSharper disable UnusedMember.Local

class VersionInf
{
    public int major { get; set; }
    public int minor { get; set; }
    public int patch { get; set; }
    public string suffix { get; set; }
    public Nullable<int> suffixVersion { get; set; }

    // "commit": "53cb9e5f535ec36b34364a2dd9ec52d1867f2214"
    public string Version() => $"{major}.{minor}.{patch}";

    public string VersionWithSuffix()
    {
        var prefix =
            suffix == null ? ""
            : suffixVersion == null ? $"-{suffix}"
            : $"-{suffix}.{suffixVersion}";
        return $"{Version()}{prefix}";
    }
}

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Solution(GenerateProjects = true)]
    // ReSharper disable once InconsistentNaming
    readonly Solution Solution;
    Project CoreProject => Solution.src.TinyTypeGen;
    Project AspNetCoreProject => Solution.src.TinyTypeGen_AspNetCore;
    Project GiraffeProject => Solution.src.TinyTypeGen_Giraffe;
    Project SignalR => Solution.src.TinyTypeGen_SignalR;

    static string GetVersion()
    {
        var versionJson = File.ReadAllText(RootDirectory / "version.json");
        var v = JsonSerializer.Deserialize<VersionInf>(versionJson);
        return v.VersionWithSuffix();
    }

    Target Clean =>
        _ =>
            _.Executes(() =>
            {
                if (!Directory.Exists(OutputDirectory))
                    return;
                foreach (var file in Directory.EnumerateFiles(OutputDirectory))
                {
                    Log.Information("Delete file {Value}", file);
                    File.Delete(file);
                }
            });

    Target Restore => _ => _.Executes(() => DotNetRestore(_ => _.SetProjectFile(CoreProject)));

    Target Compile =>
        _ =>
            _.DependsOn(Restore, Clean)
                .Executes(() =>
                    DotNetBuild(_ =>
                        _.SetProjectFile(Solution)
                            .SetConfiguration("Release")
                            .SetVersion(GetVersion())
                    )
                );

    AbsolutePath OutputDirectory = RootDirectory / "output";

    Target Pack =>
        _ =>
            _.DependsOn(Compile)
                .Executes(() =>
                {
                    var version = GetVersion();
                    DotNetPack(_ =>
                        _.SetConfiguration("Release")
                            .SetProject(CoreProject)
                            .SetVersion(version)
                            .EnableNoBuild()
                            .SetOutputDirectory(OutputDirectory)
                            .SetIncludeSymbols(true)
                            .SetVersion(GetVersion())
                    );
                    DotNetPack(_ =>
                        _.SetConfiguration("Release")
                            .SetOutputDirectory(OutputDirectory)
                            .SetProject(AspNetCoreProject)
                            .SetVersion(version)
                            .SetIncludeSymbols(true)
                    );
                    DotNetPack(_ =>
                        _.SetConfiguration("Release")
                            .SetOutputDirectory(OutputDirectory)
                            .SetProject(GiraffeProject)
                            .SetVersion(version)
                            .SetIncludeSymbols(true)
                    );
                    DotNetPack(_ =>
                        _.SetConfiguration("Release")
                            .SetOutputDirectory(OutputDirectory)
                            .SetProject(SignalR)
                            .SetVersion(version)
                            .SetIncludeSymbols(true)
                    );
                });

    [Parameter]
    // ReSharper disable once InconsistentNaming
    string NugetApiUrl = "https://api.nuget.org/v3/index.json";

    Target Publish =>
        _ =>
            _.DependsOn(Pack, Tests)
                .Requires(() => NugetApiUrl)
                .Executes(() =>
                {
                    var env = File.ReadAllText(RootDirectory / ".env.local");
                    var apiKey = env.Split("=")[1];
                    DotNetNuGetPush(_ =>
                        _.SetTargetPath(OutputDirectory / "*.nupkg")
                            .SetSource(NugetApiUrl)
                            .SetApiKey(apiKey)
                    );
                });

    Target IntegrationTests =>
        _ =>
            _.Executes(() =>
            {
                DotNetTest(_ => _.SetProjectFile(Solution.tests.IntegrationTests));
                StartProcess(
                        "pnpm",
                        arguments: "run format",
                        workingDirectory: Solution.tests.IntegrationTests.Directory / "examples"
                    )
                    .AssertZeroExitCode();
                StartProcess(
                        "pnpm",
                        arguments: "run build",
                        workingDirectory: Solution.tests.IntegrationTests.Directory / "examples"
                    )
                    .AssertZeroExitCode();
            });

    Target UnitTests =>
        _ =>
            _.Executes(() =>
            {
                DotNetTest(_ => _.SetProjectFile(Solution.tests.TinyTypeGen_Test));
            });

    private Target Format =>
        _ =>
            _.Executes(() =>
            {
                void Format(string dir)
                {
                    StartProcess("fantomas", arguments: ".", workingDirectory: dir)
                        .AssertZeroExitCode();
                }

                Format(Solution.src.TinyTypeGen.Directory);
                foreach (var project in Solution.AllProjects)
                {
                    Format(project.Directory);
                }
            });

    private Target Tests => _ => _.DependsOn(UnitTests, IntegrationTests);

    [GitRepository]
    // ReSharper disable once InconsistentNaming
    readonly GitRepository Repository;

    Target PrintGit =>
        _ =>
            _.Executes(() =>
            {
                Log.Information("Commit = {Value}", Repository.Commit);
                Log.Information("Branch = {Value}", Repository.Branch);
                Log.Information("Tags = {Value}", Repository.Tags);

                Log.Information("main branch = {Value}", Repository.IsOnMainBranch());
                Log.Information(
                    "main/master branch = {Value}",
                    Repository.IsOnMainOrMasterBranch()
                );
                Log.Information("release/* branch = {Value}", Repository.IsOnReleaseBranch());
                Log.Information("hotfix/* branch = {Value}", Repository.IsOnHotfixBranch());
                var ancestor = Git(
                        $"merge-base {GitCurrentBranch()} main",
                        workingDirectory: null,
                        logOutput: false
                    )
                    .Select(v => v.Text)
                    .Single();
                var count = Git(
                        $"rev-list {GitCurrentBranch()} ^main --pretty=oneline --count",
                        workingDirectory: null,
                        logOutput: false
                    )
                    .Select(v => v.Text)
                    .Single();
                Log.Information("Count (Height) = {Value}", count);
                Log.Information("Ancestor {Value}", ancestor);
                Log.Information("Https URL = {Value}", Repository.HttpsUrl);
                Log.Information("SSH URL = {Value}", Repository.SshUrl);
            });

    // Target Print =>
    //     _ =>
    //         _.Executes(() =>
    //         {
    //             var versionJson = File.ReadAllText(RootDirectory / "version.json");
    //             var v = JsonSerializer.Deserialize<VersionInf>(versionJson);
    //             Log.Information("NerdbankVersioning = {@Value}", v);
    //         });
}
