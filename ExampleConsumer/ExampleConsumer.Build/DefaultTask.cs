using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNet;
using Cake.Core;
using Cake.Frosting;

[TaskName("Default")]
public class DefaultTask : FrostingTask
{
    public override void Run(ICakeContext context)
    {
        context.Information("Try like .\\build.ps1 --target=Lint");
    }
}