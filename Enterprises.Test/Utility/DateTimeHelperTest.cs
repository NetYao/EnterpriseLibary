using System;
using Enterprises.Framework.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprises.Test.Utility
{
    [TestClass]
    public class DateTimeHelperTest
    {
        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void DateHelperTest()
        {
            TestContext.WriteLine(DateTime.Now.ToCnWeekDay());
            var dt=new DateTime(2014,3,4);
            TestContext.WriteLine(dt.ToCnWeekDay());
            TestContext.WriteLine(dt.GetWeekFristDay().ToShortDateString());
            TestContext.WriteLine(dt.GetWeekLastDay().ToShortDateString());
            TestContext.WriteLine(dt.GetMonthFristDay().ToShortDateString());
            TestContext.WriteLine(dt.GetMonthLastDay().ToShortDateString());
            TestContext.WriteLine(dt.GetYearFristDay().ToShortDateString());
            TestContext.WriteLine(dt.GetYearLastDay().ToShortDateString());
            TestContext.WriteLine(dt.GetQuarterFristDay().ToShortDateString());
            TestContext.WriteLine(dt.GetQuarterLastDay().ToShortDateString());
            TestContext.WriteLine(dt.GetMonthDays().ToString());




        }
    }
}
