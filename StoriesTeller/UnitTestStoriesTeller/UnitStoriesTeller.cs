using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoriesTeller;

namespace UnitTestStoriesTeller
{
    [TestClass]
    public class UnitStoriesTeller
    {
        [TestMethod]
        public void TestMethodGetStoryResource()
        {
            Function func = new Function();
            func.GetStoryFromResources("The Legendary Origins of the Viet People", null);
        }
    }
}
