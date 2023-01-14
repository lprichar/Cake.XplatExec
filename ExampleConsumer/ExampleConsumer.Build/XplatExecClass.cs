using Cake.Common;
using Cake.Core.IO;
using System.Runtime.CompilerServices;

public static class XplatExecClass
{
    public static void XplatExec(this BuildContext context, FilePath processFile, ProcessArgumentBuilder processArgumentBuilder)
    {
        if (context.IsRunningOnLinux())
        {
            throw new System.NotSupportedException("Linux support is TBD");
        } else
        {
            context.StartProcess(processFile, new ProcessSettings
            {
                Arguments = processArgumentBuilder
            });
        }
    }

    public static FilePath XplatFindRuntimeFor(this BuildContext context, FilePath executableFile)
    {
        var executableFileName = executableFile.GetFilename();
        var executableDir = executableFile.GetDirectory();
        if (context.IsRunningOnWindows()) 
        {
            return executableDir.CombineWithFilePath($"{executableFileName}.runtimeconfig.json");
        }
        else
        {
            return executableDir.CombineWithFilePath($"{executableFileName}.unix.runtimeconfig.json");
        }
    }

    private static FilePath GetRuntimeConfig(BuildContext context)
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

