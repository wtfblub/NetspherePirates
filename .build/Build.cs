using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

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
            DotNetBuild(x => x.SetWorkingDirectory(Solution.Directory)
                .SetProjectFile(Solution.Path)
                .SetConfiguration(Configuration));
        });
}
