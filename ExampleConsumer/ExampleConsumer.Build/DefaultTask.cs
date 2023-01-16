using Cake.Core;
using Cake.Frosting;

[TaskName("Default")]
// depending on LintTask is unusual, but convenient for debugging and simplicity of execution
[IsDependentOn(typeof(LintTask))]
public class DefaultTask : FrostingTask
{
    public override void Run(ICakeContext context)
    {
    }
}