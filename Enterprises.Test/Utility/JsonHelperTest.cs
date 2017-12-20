using System;
using Enterprises.Framework.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprises.Test.Utility
{
    [TestClass]
    public class JsonHelperTest
    {
        private TestContext _testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
            }
        }

        [TestMethod]
        public void IsJsonTest()
        {
            const string jsonstr = "{key:'keyName',name='Name'}";
            var result = JsonHelper.IsJson(jsonstr);
            int errorindex = 0;
            var result2 = JsonHelper.IsJson(jsonstr, out errorindex);

            TestContext.WriteLine(result.ToString());
            TestContext.WriteLine(string.Format("{0}:{1}",result2,errorindex));

            const string jsonstr2 = "{key:'keyName',name:'Name'}";
            var result3 = JsonHelper.IsJson(jsonstr2);
            const int errorindex2 = 0;
            var result4 = JsonHelper.IsJson(jsonstr2, out errorindex);

            TestContext.WriteLine(result3.ToString());
            TestContext.WriteLine(string.Format("{0}:{1}", result4, errorindex2));
        }
    }
}
