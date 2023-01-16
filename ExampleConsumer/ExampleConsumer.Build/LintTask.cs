using Cake.Common.Tools.InspectCode;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.XplatExec;

[TaskName("Lint")]
public class LintTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        InspectCodeXplatExec(context);
    }

    private void InspectCodeXplatExec(BuildContext context)
    {
        var solutionFile = new FilePath("../../ExampleProjectToLint/ExampleProjectToLint.sln");
        var settings = new InspectCodeSettings
        {
            OutputFile = new FilePath("../TestResults/Results.xml")
        };

        /*
         * PROBLEM
         * I want to run commands like JetBrains `inspectcode.exe` to perform a static analysis on my code.  I need it to run
         * both on Windows (locally) and Linux (build server / other dev machines).
         *
         * SOLUTION #1 - InspectCode
         * Fortunately there's a built in plugin that should work, like this:
         */

        //context.InspectCode(solutionFile, new InspectCodeSettings
        //{
        //    OutputFile = settings.OutputFile.FullPath,
        //    NoBuildinSettings = true
        //});

        /*
         * RESULT #1
         * Works in Windows, but fails on Linux with:
         * Executing: /usr/bin/mono "/mnt/c/dev/GitHub/Cake.XplatExec/ExampleConsumer/ExampleConsumer.Build/tools/JetBrains.ReSharper.CommandLineTools.2022.3.1/tools/inspectcode.exe" /no-buildin-settings "/mnt/c/dev/GitHub/Cake.XplatExec/ExampleConsumer/ExampleConsumer.sln"
         * Could not load signature of JetBrains.Application.HostProductInfoComponent:get_ProductAboutBoxImage due to: Could not load file or assembly 'PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' or one of its dependencies.
         * Could not load signature of JetBrains.Application.IApplicationHostImages:get_ProductAboutBoxImage due to: Could not load file or assembly 'PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' or one of its dependencies.
         * The problem appears to be because it's attempting to run it in Mono, but shouldn't be.
         * I've got Mono and .NET SDK 6 and 7.  It's probably easy to fix, but I
         * see errors in plugins like this all the time, so I'm interested in a more
         * flexible and lower level option anyway.
         *
         * SOLUTION #2 - DotNetExecute
         * The binary I need `inspectcode.exe` is downloaded via the command
         * `.InstallTool(new System.Uri("nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2022.3.1"))`
         * in Program.cs.  By playing around I know it works on Windows and Mac, but require different syntax as follows:
         *
         * PC: dotnet exec --runtimeconfig "./ExampleConsumer.Build/tools/JetBrains.ReSharper.CommandLineTools.2022.3.1/tools/inspectcode.runtimeconfig.json" "./ExampleConsumer.Build/tools/JetBrains.ReSharper.CommandLineTools.2022.3.1/tools/inspectcode.exe" --build -o="../TestResults/lint-results.xml" ../ExampleProjectToLint/ExampleProjectToLint.sln
         * Linux: dotnet exec --runtimeconfig "./ExampleConsumer.Build/tools/JetBrains.ReSharper.CommandLineTools.2022.3.1/tools/inspectcode.unix.runtimeconfig.json" "./ExampleConsumer.Build/tools/JetBrains.ReSharper.CommandLineTools.2022.3.1/tools/inspectcode.exe" --build -o="../TestResults/lint-results.xml" ../ExampleProjectToLint/ExampleProjectToLint.sln
         *
         * So I might try to execute it with DotNetExecute as below:
         */

        //context.DotNetExecute(inspectCodeExe, new ProcessArgumentBuilder()
        //    .Append($"--output={settings.OutputFile.FullPath}")
        //    .Append("--nobuild")
        //    .Append(solutionFile.FullPath));

        /*
         * That works on Windows, but fails on Linux because there's no way to specify
         * the --runtimeconfig, which seems to be required on Linux and optional on Windows e.g. I need
         * `dotnet run --runtimeconfig inspectcode.unix.runtimeconfig.json inspectcode.exe [params]`
         * NOT `dotnet inspectcode.exe --runtimeconfig inspectcode.unix.runtimeconfig.json [params]`
         *
         * #3 XplatExec
         * I get frustrated when I know exactly what I want to run and how I want to run it but the existing
         * plugins don't give me enough control. I could use `context.StartAndReturnProcess`, but it requires
         * a lot of code to get right in a cross platform way.  Thus XplatExec: a super simple helper for
         * running raw commands at the terminal with maximum control.  So here's a really low level way of
         * solving the problem with it:
         */

        var inspectCodeExe = context.Tools.Resolve("inspectcode.exe");
        var runtimeConfig = context.XplatFindRuntimeFor(inspectCodeExe);

        context.XplatExec("dotnet", new ProcessArgumentBuilder()
            .Append("exec")
            .Append("--runtimeconfig")
            .Append(runtimeConfig.FullPath)
            .Append(inspectCodeExe.FullPath)
            .Append($"--output={settings.OutputFile.FullPath}")
            .Append("--build")
            .Append(solutionFile.FullPath));
    }
}

