using System.Globalization;
using System.IO;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

internal class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' or 'Release'")]
    public readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    [Solution("NetspherePirates.sln")]
    public readonly Solution Solution;

    public Target Clean => _ => _
        .Executes(() =>
        {
            DotNetClean(x => x.SetWorkingDirectory(Solution.Directory)
                .SetProject(Solution.Path));
            var dist = Path.Combine(Solution.Directory, "dist");
            if (Directory.Exists(dist))
                Directory.Delete(dist, true);
        });

    public Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(x => x.SetWorkingDirectory(Solution.Directory)
                .SetProjectFile(Solution.Path));
        });

    public Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            var configuration = Configuration;
            switch (configuration.ToLower())
            {
                case "release":
                    configuration = "ReleaseNoTools";
                    break;

                case "debug":
                    configuration = "DebugNoTools";
                    break;
            }

            DotNetBuild(x => x.SetWorkingDirectory(Solution.Directory)
                .SetProjectFile(Solution.Path)
                .SetConfiguration(configuration));
        });

    public Target Publish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var configuration = Configuration;
            switch (configuration.ToLower())
            {
                case "release":
                    configuration = "ReleaseNoTools";
                    break;

                case "debug":
                    configuration = "DebugNoTools";
                    break;
            }

            DotNetPublish(x => x.SetWorkingDirectory(Solution.Directory)
                .SetProject(Solution.Path)
                .SetConfiguration(configuration));
        });

    public Target Tools => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            var configuration = Configuration;
            switch (configuration.ToLower())
            {
                case "release":
                    configuration = "ReleaseOnlyTools";
                    break;

                case "debug":
                    configuration = "DebugOnlyTools";
                    break;
            }

            MSBuild(x => x.SetWorkingDirectory(Solution.Directory)
                .SetProjectFile(Solution.Path)
                .SetConfiguration(configuration));
        });

    public Target Dist => _ => _
        .DependsOn(Publish)
        .DependsOn(Tools)
        .Executes(() =>
        {
            var projects = new (string ProjectPath, string Output, string Framework, bool publish)[]
            {
                ("src/Netsphere.Server.Auth", "Auth", "netcoreapp2.1", true),
                ("src/Netsphere.Server.Chat", "Chat", "netcoreapp2.1", true),
                ("src/Netsphere.Server.Game", "Game", "netcoreapp2.1", true),
                ("src/Netsphere.Server.Relay", "Relay", "netcoreapp2.1", true),
                ("src/tools/DataExtractor", "tools", "net472", false),
                ("src/tools/NetsphereExplorer", "tools", "net472", false),
                ("src/tools/Netsphere.Tools.ShopEditor", "tools/ShopEditor", "netcoreapp2.1", true)
            };

            var dist = Path.Combine(Solution.Directory, "dist");
            if (Directory.Exists(dist))
                Directory.Delete(dist, true);

            foreach (var project in projects)
            {
                var build = Path.Combine(Solution.Directory, project.ProjectPath, "bin", Configuration, project.Framework);
                if (project.publish)
                    build = Path.Combine(build, "publish");

                var output = Path.Combine(Solution.Directory, "dist", project.Output);
                if (!Directory.Exists(output))
                    Directory.CreateDirectory(output);

                foreach (var file in Directory.EnumerateFiles(build, "*", SearchOption.TopDirectoryOnly))
                    File.Copy(file, Path.Combine(output, Path.GetFileName(file)));

                var buildDir = new DirectoryInfo(build);
                foreach (var dir in buildDir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                    dir.MoveTo(Path.Combine(output, dir.Name));
            }
        });
}
