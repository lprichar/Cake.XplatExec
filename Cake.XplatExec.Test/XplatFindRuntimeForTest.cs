using Cake.Common;
using NSubstitute;
using Cake.Core.IO;
using Cake.Frosting;
using Shouldly;
using System;
using Cake.Core;

namespace Cake.XplatExec.Test
{
    [TestClass]
    public class XplatFindRuntimeForTest
    {
        [TestMethod]
        public void GivenJetbrainsOnWindows_WhenXplatFindRuntimeFor_ItReturnsCmdRuntime()
        {
            // ARRANGE
            var frostingContext = GetContext(isRunningOnWindows: true);

            // ACT
            var runtimeFile = XplatExecHelper.XplatFindRuntimeFor(frostingContext,
                new FilePath("/tools/JetBrains.ReSharper.CommandLineTools.2022.3.1/tools/inspectcode.exe"));
            
            // ASSERT
            runtimeFile.FullPath.ShouldBe("/tools/JetBrains.ReSharper.CommandLineTools.2022.3.1/tools/inspectcode.runtimeconfig.json");
        }

        [TestMethod]
        public void GivenJetbrainsOnLinux_WhenXplatFindRuntimeFor_ItReturnsCmdRuntimeWithUnixInName()
        {
            // ARRANGE
            var frostingContext = GetContext(isRunningOnWindows: false);

            // ACT
            var runtimeFile = XplatExecHelper.XplatFindRuntimeFor(frostingContext,
                new FilePath("/tools/JetBrains.ReSharper.CommandLineTools.2022.3.1/tools/inspectcode.exe"));

            // ASSERT
            runtimeFile.FullPath.ShouldBe("/tools/JetBrains.ReSharper.CommandLineTools.2022.3.1/tools/inspectcode.unix.runtimeconfig.json");
        }

        private static IFrostingContext GetContext(bool isRunningOnWindows)
        {
            var frostingContext = Substitute.For<IFrostingContext>();
            var environment = Substitute.For<ICakeEnvironment>();
            var cakePlatform = Substitute.For<ICakePlatform>();

            var platformFamily = isRunningOnWindows ? PlatformFamily.Windows : PlatformFamily.Linux;
            cakePlatform.Family.Returns(platformFamily);
            environment.Platform.Returns(cakePlatform);
            frostingContext.Environment.Returns(environment);
            return frostingContext;
        }
    }
}
