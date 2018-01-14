using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoriesTeller;

namespace UnitTestStoriesTeller
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Function func = new Function();
            func.GetStoryFromResources("The Legendary Origins of the Viet People", null);
        }
    }
}
