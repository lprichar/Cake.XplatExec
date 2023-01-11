using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .InstallTool(new System.Uri("nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2022.3.1"))
            .Run(args);
    }
}

[TaskName("Lint")]
public class LintTask : FrostingTask
{
    public override void Run(ICakeContext context)
    {
        context.Information("Running CommandLineTools");
    }
}
