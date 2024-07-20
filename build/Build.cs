using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class VersionInf
{
    // ReSharper disable once InconsistentNaming
    public string version { get; set; }
}

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Solution(GenerateProjects = true)] readonly Solution Solution;
    Project CoreProject => Solution.src.TinyTypeGen;
    Project AspNetCoreProject => Solution.src.TinyTypeGen_AspNetCore;
    Project GiraffeProject => Solution.src.TinyTypeGen_Giraffe;

    static string GetVersion()
    {
        var versionJson = File.ReadAllText(RootDirectory / "version.json");
        var v = JsonSerializer.Deserialize<VersionInf>(versionJson);
        var version = v.version.Split("-");
        var versionPrefix = version[0];
        var versionSuffix = version.Length > 1 ? "-" + version[1] : null;
        var ancestor = GitTasks.Git($"merge-base {GitTasks.GitCurrentBranch()} main", workingDirectory: null, logOutput: false)
            .Select(v => v.Text)
            .Single();
        var count = GitTasks.Git($"rev-list {GitTasks.GitCurrentBranch()} ^main --pretty=oneline --count", workingDirectory: null, logOutput: false)
            .Select(v => v.Text)
            .Single();
        var result = $"{versionPrefix}.0.{count}{versionSuffix}";
        Log.Information("version = {Value}", result);
        return result;
    }

    Target Clean => _ => _.Executes(() =>
    {
        if (Directory.Exists(OutputDirectory))
        {
            foreach (var file in Directory.EnumerateFiles(OutputDirectory))
            {
                Log.Information("file {value}", file);
                File.Delete(file);
            }
        }
    });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(_ => _.SetProjectFile(CoreProject));
        });

    Target Compile => _ => _
        .DependsOn(Restore, Clean)
        .Executes(() =>
        {
            var version = GetVersion();
            DotNetTasks.DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration("Release")
                .SetVersion(version)
            );
        });

    AbsolutePath OutputDirectory = RootDirectory / "output";

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var version = GetVersion();
            DotNetTasks.DotNetPack(_ => _
                .SetConfiguration("Release")
                .SetProject(CoreProject)
                .SetVersion(version)
                .EnableNoBuild()
                .SetOutputDirectory(OutputDirectory)
                .SetIncludeSymbols(true)
                .SetVersion(GetVersion())
            );
            DotNetTasks.DotNetPack(_ => _
                .SetConfiguration("Release")
                .SetOutputDirectory(OutputDirectory)
                .SetProject(AspNetCoreProject)
                .SetVersion(version)
                .SetIncludeSymbols(true));
            DotNetTasks.DotNetPack(_ => _
                .SetConfiguration("Release")
                .SetOutputDirectory(OutputDirectory)
                .SetProject(GiraffeProject)
                .SetVersion(version)
                .SetIncludeSymbols(true)
            );
        });

    [Parameter] string NugetApiUrl = "https://api.nuget.org/v3/index.json";

    Target Publish => _ => _
        .DependsOn(Pack, Test)
        .Requires(() => NugetApiUrl)
        .Executes(() =>
        {
            var env = File.ReadAllText(RootDirectory / ".env.local");
            var apiKey = env.Split("=")[1];
            DotNetTasks.DotNetNuGetPush(_ => _
                .SetTargetPath(OutputDirectory / "*.nupkg")
                .SetSource(NugetApiUrl)
                .SetApiKey(apiKey)
            );
        });

    Target Test => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(_ => _
                .SetProjectFile(Solution.TinyTypeGen_Test)
            );
        });

    [GitRepository] readonly GitRepository Repository;

    Target PrintGit => _ => _
        .Executes(() =>
        {
            Log.Information("Commit = {Value}", Repository.Commit);
            Log.Information("Branch = {Value}", Repository.Branch);
            Log.Information("Tags = {Value}", Repository.Tags);

            Log.Information("main branch = {Value}", Repository.IsOnMainBranch());
            Log.Information("main/master branch = {Value}", Repository.IsOnMainOrMasterBranch());
            Log.Information("release/* branch = {Value}", Repository.IsOnReleaseBranch());
            Log.Information("hotfix/* branch = {Value}", Repository.IsOnHotfixBranch());
            var ancestor = GitTasks.Git($"merge-base {GitTasks.GitCurrentBranch()} main", workingDirectory: null, logOutput: false)
                .Select(v => v.Text)
                .Single();
            var count = GitTasks.Git($"rev-list {GitTasks.GitCurrentBranch()} ^main --pretty=oneline --count", workingDirectory: null, logOutput: false)
                .Select(v => v.Text)
                .Single();
            Log.Information("Count (Height) = {Value}", count);
            Log.Information("Anchestor {value}", ancestor);
            Log.Information("Https URL = {Value}", Repository.HttpsUrl);
            Log.Information("SSH URL = {Value}", Repository.SshUrl);
        });

    Target Print => _ => _
        .Executes(() =>
        {
            var versionJson = File.ReadAllText(RootDirectory / "version.json");
            var v = JsonSerializer.Deserialize<VersionInf>(versionJson);
            Log.Information("NerdbankVersioning = {@Value}", v);
        });
}
