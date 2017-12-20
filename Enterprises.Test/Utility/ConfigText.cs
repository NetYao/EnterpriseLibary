using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprises.Framework.Plugin.Config;
using Enterprises.Framework.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enterprises.Test.Utility
{
    [TestClass]
    public class ConfigText
    {
        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void ConfigFactorytTest()
        {
            var factory = ConfigFactory.Create("ConfigText.config");
            TestContext.WriteLine(factory.GetAppSettings("LoginServiceBusPassword",false));
            var sec = factory.GetConfigSections("GoogleMailSetting");
            foreach (var configSection in sec)
            {
                TestContext.WriteLine(configSection.SectionType);
            }
        }

        [TestMethod]
        public void ConfigUitilyTest()
        {
            TestContext.WriteLine(ConfigUtility.GetFrameworkConfigValue("LoginServiceBusPassword",""));
            var sec = ConfigUtility.GetFrameworkConfigSections("GoogleMailSetting","邮箱配置");
            foreach (var configSection in sec)
            {
                TestContext.WriteLine(configSection.SectionType);
            }

            TestContext.WriteLine(ConfigUtility.GetSystemConfigValue("log4net.Config","log4日志",false));
            TestContext.WriteLine(ConfigUtility.GetSystemConnectionStringsConfigValue("SHIPING_OA", "log4日志", false));
        }
    }
}
