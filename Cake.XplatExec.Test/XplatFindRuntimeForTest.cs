namespace Cake.XplatExec.Test
{
    [TestClass]
    public class XplatFindRuntimeForTest
    {
        [TestMethod]
        public void GivenJetbrainsOnWindows_WhenXplatFindRuntimeFor_ItReturnsCmdRuntime()
        {
            // todo: mock IBuildContext
            Assert.AreEqual(3, 1 + 2);
        }
    }
}
