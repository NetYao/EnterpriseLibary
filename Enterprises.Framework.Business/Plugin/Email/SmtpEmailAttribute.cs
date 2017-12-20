using System;

namespace Enterprises.Framework.Email
{

    #region 定制发送邮件的插件属性值

    /// <summary>
    /// 定制发送邮件的插件属性值
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class SmtpEmailAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        public SmtpEmailAttribute(string name)
        {
            PlugInName = name;
        }

        /// <summary>
        /// 插件名称
        /// </summary>
        public string PlugInName { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// DLL 文件名称
        /// </summary>
        public string DllFileName { get; set; }
    }
    #endregion

}
