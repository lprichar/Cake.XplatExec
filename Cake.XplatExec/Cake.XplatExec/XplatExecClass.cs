using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Core.IO;
using Cake.Core.IO.Arguments;
using Cake.Frosting;

public static class XplatExecClass
{
    // todo: 1. Extract Interface
    // 2. move to other file
    public static void XplatExec(this FrostingContext context, string cmd, ProcessArgumentBuilder processArgumentBuilder)
    {
        using var process = XplatExec(context, cmd, new ProcessSettings { Arguments = processArgumentBuilder });
        process.WaitForExit();
        var standardOut = process.GetStandardOutput().ToList();
        context.Information(string.Join("\n", standardOut));
        var exitCode = process.GetExitCode();
        if (exitCode != 0)
        {
            throw new Exception($"Exit code: {exitCode}");
        }
    }

    private static IProcess XplatExec(FrostingContext context, string cmd, ProcessSettings settings)
    {
        if (context.IsRunningOnWindows())
        {
            settings.Arguments.Prepend(new TextArgument(cmd));
            context.Information($"Executing `powershell {string.Join(" ", settings.Arguments.ToList())}`");
            return context.StartAndReturnProcess("powershell", settings);
        }

        context.Information($"Running `{cmd} {string.Join(" ", settings.Arguments.ToList())}`");
        return context.StartAndReturnProcess(cmd, settings);
    }

    public static FilePath XplatFindRuntimeFor(this FrostingContext context, FilePath executableFile)
    {
        var executableFileName = executableFile.GetFilename();
        var fileNameWithoutExtension = executableFileName.GetFilename().GetFilenameWithoutExtension();
        var executableDir = executableFile.GetDirectory();
        if (context.IsRunningOnWindows()) 
        {
            return executableDir.CombineWithFilePath($"{fileNameWithoutExtension}.runtimeconfig.json");
        }
        else
        {
            return executableDir.CombineWithFilePath($"{fileNameWithoutExtension}.unix.runtimeconfig.json");
        }
    }

    private static FilePath GetRuntimeConfig(FrostingContext context)
    {
        if (context.IsRunningOnLinux())
        {
            return "inspectcode.unix.runtimeconfig.json";
        }
        else
        {
            return "inspectcode.runtimeconfig.json";
        }
    }
}

