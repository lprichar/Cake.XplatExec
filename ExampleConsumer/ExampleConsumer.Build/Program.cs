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

