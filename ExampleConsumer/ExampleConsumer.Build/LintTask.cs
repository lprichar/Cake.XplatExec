using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.InspectCode;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

[TaskName("Lint")]
public class LintTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        InspectCodeXplatExec(context);
        InspectCodeDotNetExec(context);
    }

    private void InspectCodeDotNetExec(BuildContext context)
    {
        // context.DotNetExecute(...)
    }

    private void InspectCodeXplatExec(BuildContext context)
    {
        var solutionFile = new FilePath("../../ExampleConsumer.sln");
        var settings = new InspectCodeSettings();
        var inspectCodeExe = context.Tools.Resolve("inspectcode.exe");
        var runtimeConfig = context.XplatFindRuntimeFor(inspectCodeExe);

        context.XplatExec(inspectCodeExe, new ProcessArgumentBuilder()
            .Append("dotnet")
            .Append("exec")
            .Append("--runtimeconfig")
            .Append(runtimeConfig.FullPath)
            .Append($"--output={settings.OutputFile.FullPath}")
            .Append("--nobuild")
            .Append(solutionFile.FullPath));
    }
}

