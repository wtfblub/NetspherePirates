using System.IO;
using System.Linq;
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
        .Before(Restore, Compile, Publish, Tools, Dist)
        .Executes(() =>
        {
            DotNetClean(x => x.SetWorkingDirectory(Solution.Directory)
                .SetProject(Solution.Path));
            var dist = Path.Combine(Solution.Directory, "dist");
            if (Directory.Exists(dist))
                Directory.Delete(dist, true);
        });

    public Target Restore => _ => _
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
        .DependsOn(Clean, Tools, Publish)
        .Executes(() =>
        {
            var projects = new (string ProjectPath, string Output, string Framework, bool publish, string[] filesToCopy)[]
            {
                ("src/Netsphere.Server.Auth", "Auth", "netcoreapp2.1", true, null),
                ("src/Netsphere.Server.Chat", "Chat", "netcoreapp2.1", true, null),
                ("src/Netsphere.Server.Game", "Game", "netcoreapp2.1", true, null),
                ("src/Netsphere.Server.Relay", "Relay", "netcoreapp2.1", true, null),
                ("src/tools/DataExtractor", "tools", "net472", false, null),
                ("src/tools/NetsphereExplorer", "tools", "net472", false, null),
                ("src/tools/Netsphere.Tools.ShopEditor", "tools/ShopEditor", "netcoreapp2.1", true, null),
                ("src/plugins/ExamplePlugin", "plugins/ExamplePlugin", "netcoreapp2.1", true, new[]
                {
                    "ExamplePlugin.deps.json", "ExamplePlugin.dll", "ExamplePlugin.pdb"
                }),
                ("src/plugins/WebApi", "plugins/WebApi", "netcoreapp2.1", true, new[]
                {
                    "WebApi.deps.json", "WebApi.dll", "WebApi.pdb", "webapi.hjson",
                    "Unosquare.Swan.Lite.dll",
                    "Unosquare.Labs.EmbedIO.dll"
                })
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

                if (project.filesToCopy != null)
                {
                    foreach (var file in project.filesToCopy)
                        File.Copy(Path.Combine(build, file), Path.Combine(output, file));
                }
                else
                {
                    foreach (var file in Directory.EnumerateFiles(build, "*", SearchOption.TopDirectoryOnly))
                        File.Copy(file, Path.Combine(output, Path.GetFileName(file)));

                    var buildDir = new DirectoryInfo(build);
                    foreach (var dir in buildDir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                        dir.MoveTo(Path.Combine(output, dir.Name));
                }
            }

            File.WriteAllText(Path.Combine(dist, "tools", "ShopEditor.bat"),
                "dotnet ShopEditor\\Netsphere.Tools.ShopEditor.dll");

            File.WriteAllText(Path.Combine(dist, "tools", "ShopEditor.sh"),
                "#!/bin/sh\ndotnet ShopEditor/Netsphere.Tools.ShopEditor.dll");
        });
}
