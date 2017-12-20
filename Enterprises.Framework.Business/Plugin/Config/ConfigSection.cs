using System;

namespace Enterprises.Framework.Plugin.Config
{
    /// <summary>
    /// 扩展配置文件ConfigSection配置块,可以按组进行分类
    //  如：设计邮件相关配置章节
    // <configSections>
    //  <sectionGroup name ="GoogleMailSetting" type ="Google Mail的相关配置">
    //    <section name="EMailSenderName" type="Domo.Yao"/>
    //    <section name="EMailSenderAddress" type="Domo.Yao@gmail.com"/>
    //    <section name="EMailSenderAccount" type="Domo.Yao"/>
    //    <section name="EMailSenderPassword" type="***"/>
    //    <section name="EMailPop3Server" type="imap.gmail.com"/>
    //    <section name="EMailPop3ServerPort" type="993"/>
    //    <section name="EMailSmtpServer" type="smtp.gmail.com"/>
    //    <section name="EMailSmptServerPort" type="587"/>
    //    <section name="EMailSmptNeedCheck" type="true"/>
    //    <section name="EMailEnableSSL" type="true"/>
    //  </sectionGroup>
    //</configSections>
    /// </summary>
    [Serializable]
    public class ConfigSection
    {
        private readonly string _fSectionName = string.Empty;
        private readonly string _fSectionType = string.Empty;

        /// <summary>
        ///  章节配置
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="sectionType"></param>
        public ConfigSection(string sectionName, string sectionType)
        {
            _fSectionName = sectionName;
            _fSectionType = sectionType;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string SectionName
        {
            get
            {
                return _fSectionName;
            }
        }

        /// <summary>
        /// 类型值
        /// </summary>
        public string SectionType
        {
            get
            {
                return _fSectionType;
            }
        }
    }
}