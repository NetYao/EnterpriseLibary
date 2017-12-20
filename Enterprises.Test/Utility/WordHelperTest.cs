using System;
using Enterprises.Framework.Plugin.Office;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Enterprises.Test.Utility
{
    [TestClass]
    public class WordHelperTest
    {
        private TestContext _testContextInstance;

        [TestMethod]
        public void TestReplaceBookMark()
        {
            var missing = Type.Missing;
            var wordHelper = new WordHelper();
            var isOpen=wordHelper.OpenAndActive(@"D:\yibiyi\FileRoot\销售合同模板-安全.docx", false, false);
            if (isOpen)
            {
                wordHelper.ReplaceBookMark("ContractCode", "合同编号");
            }

            wordHelper.Save();
            wordHelper.Close();
        }
    }
}
