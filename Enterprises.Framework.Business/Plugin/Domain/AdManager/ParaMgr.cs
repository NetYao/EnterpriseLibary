using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Enterprises.Framework.Plugin.Domain.AdManager
{
    /// <summary>
    /// 定义默认参数
    /// 参数应当保存在配置文件中；如果没有设置，则应当手动设置。
    /// </summary>
    public class ParaMgr
    {
        private ParaMgr() { }

        private static object s_lock = new object();
        private static volatile ParaMgr s_value = null;

        private static ParaMgr Value
        {
            get 
            {
                if (s_value == null)
                {
                    lock (s_lock)
                    {
                        if (s_value == null)
                        {
                            s_value = new ParaMgr();
                            NameValueCollection setting =  ConfigurationManager.AppSettings;
                            // TODO:对以下设置（AD_Admin，AD_Password）进行加密，此时应当解密。
                            if (!String.IsNullOrEmpty(setting["AD_Admin"]))
                                s_value.userName = setting["AD_Admin"];
                            if (!String.IsNullOrEmpty(setting["AD_Domain"]))
                                s_value.domain = setting["AD_Domain"];
                            if (!String.IsNullOrEmpty(setting["AD_Domain2000"]))
                                s_value.domain2000 = setting["AD_Domain2000"];
                            if (!String.IsNullOrEmpty(setting["AD_Password"]))
                                s_value.password = setting["AD_Password"];
                            if (!String.IsNullOrEmpty(setting["AD_Path"]))
                                s_value.fullPath = setting["AD_Path"];
                        }
                    }
                }

                return s_value;
            }
        }

        /// <summary>
        /// 定义ADsPath前缀。
        /// </summary>
        public const string LDAP_IDENTITY = "LDAP://";

        private string userName;        // AD 用户名，eg:administrator
        private string domain;          // Domain Name，eg:maodou.com
        private string domain2000;      // Domain Name 2000，eg:maodou
        private string password;        // AD 用户密码
        private string fullPath;        // Domain DN，eg:DC=maodou,DC=com，可以和maodou.com互相转化


        /// <summary>
        /// 设置系统参数
        /// </summary>
        /// <param name="userName">AD 用户名</param>
        /// <param name="domain">AD 域名称</param>
        /// <param name="password">AD 用户密码</param>
        /// <param name="fullPath">AD 域路径（DN）</param>
        public static void SetSysPara(string userName, string domain, string password, string fullPath)
        {
            if (!String.IsNullOrEmpty(userName))
            {
                Value.userName = userName;
            }
            if (!String.IsNullOrEmpty(domain))
            {
                Value.domain = domain;
            }
            if (!String.IsNullOrEmpty(password))
            {
                Value.password = password;
            }
            if (!String.IsNullOrEmpty(userName))
            {
                Value.fullPath = fullPath;
            }
        }

        /// <summary>
        /// 获取AD 用户名
        /// </summary>
        public static string ADUserName
        {
            get { return Value.userName; }
        }

        /// <summary>
        /// 获取AD 域名称
        /// </summary>
        public static string ADDomain
        {
            get { return Value.domain; }
        }

        /// <summary>
        /// 获取AD 域名称2000
        /// </summary>
        public static string ADDomain2000
        {
            get { return Value.domain2000; }
        }

        /// <summary>
        /// 获取AD 用户密码
        /// </summary>
        public static string ADPassword
        {
            get { return Value.password; }
        }

        /// <summary>
        /// 获取AD 域路径（DN）
        /// </summary>
        public static string ADFullPath
        {
            get { return Value.fullPath; }
        }

        /// <summary>
        /// 获取AD 用户名
        /// </summary>
        public static string ADFullName
        {
            get { return String.Format("{0}@{1}", Value.userName, Value.domain); }
        }
        
    }
}
