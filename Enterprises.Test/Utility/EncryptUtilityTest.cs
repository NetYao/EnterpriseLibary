using System;
using Enterprises.Framework.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprises.Test.Utility
{
    [TestClass]
    public class EncryptUtilityTest
    {
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void TestMethod1()
        {
            var tripleStr = EncryptUtility.EncryptByTriple("password");
            TestContext.WriteLine("EncryptByTriple="+EncryptUtility.EncryptByTriple("password"));
            TestContext.WriteLine("EncryptByTriple=" + EncryptUtility.DecryptByTriple(tripleStr));

            var md5 = EncryptUtility.EncryptByMD5("password");
            TestContext.WriteLine("EncryptByMD5=" + EncryptUtility.EncryptByMD5("password"));

            var comStr = EncryptUtility.EncryptByTriple("password","加密字符串");
            TestContext.WriteLine("EncryptByTriple=" + comStr);
            
        }
    }
}
