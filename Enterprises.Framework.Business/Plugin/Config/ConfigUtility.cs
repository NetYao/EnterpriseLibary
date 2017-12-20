using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Web;
using Enterprises.Framework.Plugin.Config;


namespace Enterprises.Framework.Utility
{
	/// <summary>
	/// 对配置文件进行操作的类
	/// </summary>
	public class ConfigUtility
    {
        /// <summary>
        /// 获取配置文件中的appSettings节中的配置内容
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetFrameworkConfigValue(string appSettingKey, string message)
        {
            return GetFrameworkConfigValue(appSettingKey, message, false);
        }

        /// <summary>
        /// 获取Enterprises.Framework.config配置文件中的appSettings节中的配置内容，如果没有配置并且允许为空则返回string.Empty
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <param name="message"></param>
        /// <param name="canEmpty"></param>
        /// <returns></returns>
        public static string GetFrameworkConfigValue(string appSettingKey,string message,bool canEmpty)
        {
            string value = FrameworkConfiguration.FrameworkConfig.GetAppSettings(appSettingKey, canEmpty);
            if (!canEmpty)
            {
                if (string.IsNullOrEmpty(value))
                {
                    var ex = new Exception(string.Format("Enterprises.Framework.config不存在{0}配置信息", appSettingKey));
                    throw ex;
                }
            }

            return value ?? (value = string.Empty);
        }

        /// <summary>
        /// 判断Framework是否存在配置节
        /// </summary>
        /// <param name="sectionGroupName">配置节名称</param>
        /// <returns></returns>
        public static bool ExistsFrameworkConfigSections(string sectionGroupName)
        {
            var result = FrameworkConfiguration.FrameworkConfig.GetConfigSections(sectionGroupName);
            if (result == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取Framework配置文件中定义的配置节内容，可以通过配置节的名称不同来重复配置某些项目
        /// </summary>
        /// <param name="sectionGroupName">配置节名称</param>
        /// <param name="sectionGroupHintMessage">配置节没有配置时的提示信息</param>
        /// <returns></returns>
        public static List<ConfigSection> GetFrameworkConfigSections(string sectionGroupName, string sectionGroupHintMessage)
        {
            var result = FrameworkConfiguration.FrameworkConfig.GetConfigSections(sectionGroupName);
            if (result == null)
            {
                var ex = new Exception(string.Format("请在Enterprises.Framework.config中的configuration/configSections段下配置name={0}的sectionGroup节点：{1}", sectionGroupName, sectionGroupHintMessage));
                throw ex;
            }

            return result;
        }

      
        /// <summary>
        /// 获取系统默认的配置文件中的appSettings节中的配置内容（如果允许为空，并且当前没有设置，则默认为true）
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <param name="message"></param>
        /// <param name="canEmpty"></param>
        /// <returns></returns>
        public static bool GetFrameworkConfigBoolValue(string appSettingKey, string message, bool canEmpty)
        {
            string value = GetFrameworkConfigValue(appSettingKey, message, canEmpty).ToUpper().Trim();

            //如果允许为空，并且当前没有设置，则默认为true
            if (canEmpty && value.Trim() == string.Empty)
            {
                return true;
            }
            return value == "TRUE";
        }

        /// <summary>
        /// 获取系统默认的配置文件中的appSettings节中的配置内容
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <param name="message"></param>
        /// <param name="canEmpty"></param>
        /// <returns></returns>
        public static int GetFrameworkConfigIntValue(string appSettingKey, string message, bool canEmpty)
        {
            string value = GetFrameworkConfigValue(appSettingKey, message, canEmpty).ToUpper().Trim();
            int result = 0;
            int.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// 获取系统默认的配置文件中的appSettings节中的配置内容
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <param name="message"></param>
        /// <param name="canEmpty"></param>
        /// <returns></returns>
        public static Guid GetFrameworkConfigGuidValue(string appSettingKey, string message, bool canEmpty)
        {
            string value = GetFrameworkConfigValue(appSettingKey, message, canEmpty).ToUpper().Trim();
            Guid result;
            bool ok = Guid.TryParse(value, out result);
            if (ok)
            {
                return result;
            }

            var ex = new Exception(string.Format("获取web.confg/app.config中的配置信息并转化为Guid时失败。Key='{0}'，说明为：{1}，请检查配置的值是否为合法的Guid", appSettingKey, message));
            throw ex;
        }


        /// <summary>
        /// 获取系统默认的配置文件中的appSettings节中的配置内容
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <param name="message"></param>
        /// <param name="canEmpty"></param>
        /// <returns></returns>
        public static bool GetSystemConfigBoolValue(string appSettingKey, string message, bool canEmpty)
        {
            string value = GetSystemConfigValue(appSettingKey, message, canEmpty).ToUpper().Trim();
            return value == "TRUE";
        }

        /// <summary>
        /// 获取系统默认的配置文件中的appSettings节中的配置内容
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <param name="message"></param>
        /// <param name="canEmpty"></param>
        /// <returns></returns>
        public static Guid GetSystemConfigGuidValue(string appSettingKey, string message, bool canEmpty)
        {
            string value = GetSystemConfigValue(appSettingKey, message, canEmpty).ToUpper().Trim();
            Guid result;
            bool ok = Guid.TryParse(value, out result);
            if (ok)
            {
                return result;
            }

            var ex = new Exception(string.Format("获取web.confg/app.config中的配置信息并转化为Guid时失败。Key='{0}'，说明为：{1}，请检查配置的值是否为合法的Guid", appSettingKey, message));
            throw ex;
        }

        /// <summary>
        /// 获取系统默认的配置文件中的appSettings节中的配置内容
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <param name="message"></param>
        /// <param name="canEmpty"></param>
        /// <returns></returns>
        public static int GetSystemConfigIntValue(string appSettingKey, string message, bool canEmpty)
        {
            string value = GetSystemConfigValue(appSettingKey, message, canEmpty).ToUpper().Trim();
            int result = 0;
            int.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// 获取系统默认的配置文件中的appSettings节中的配置内容，如果没有配置并且允许为空则返回string.Empty
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <param name="message"></param>
        /// <param name="canEmpty"></param>
        /// <returns></returns>
        public static string GetSystemConfigValue(string appSettingKey, string message, bool canEmpty)
        {
            string value = System.Configuration.ConfigurationManager.AppSettings[appSettingKey];
            if (!canEmpty)
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception(string.Format("请在Web.Config或App.Config中的configuration/appSettings段下配置Key={0}的节点，Value允许的值为：{1}.", appSettingKey, message));
                }
            }

            return value ?? (value = string.Empty);
        }

        /// <summary>
        /// 获取配置文件数据库连接
        /// </summary>
        /// <param name="appSettingKey"></param>
        /// <param name="message"></param>
        /// <param name="canEmpty"></param>
        /// <returns></returns>
        public static string GetSystemConnectionStringsConfigValue(string appSettingKey, string message, bool canEmpty)
        {
            ConnectionStringSettings css = System.Configuration.ConfigurationManager.ConnectionStrings[appSettingKey];
            if (css == null)
            {
                throw new Exception(string.Format("请在Web.Config或App.Config中的configuration/connectionStrings段下配置name={0}的节点，Value允许的值为：{1}.", appSettingKey, message));
            }

            string value = css.ConnectionString;
            if (!canEmpty)
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception(string.Format("请在Web.Config或App.Config中的configuration/connectionStrings段下配置name={0}的节点，Value允许的值为：{1}.", appSettingKey, message));
                }
            }

            return value ?? (value = string.Empty);
        }

	}
}
