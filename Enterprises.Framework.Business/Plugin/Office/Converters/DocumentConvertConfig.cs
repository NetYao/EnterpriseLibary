using System;
using System.Configuration;

namespace Enterprises.Framework.Plugin.Office.Converters
{
    /// <summary>
    /// 文档转换配置文件
    /// </summary>
    public class DocumentConvertConfig
    {
        /// <summary>
        /// 日志文件
        /// </summary>
        public static string LogFile
        {
            get
            {
                string path = ConfigurationManager.AppSettings["LogFile"];
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }
                return "C:\\log.txt";
            }
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public static string ConnectionString
        {
            get
            {
                string connectionStringName = ConfigurationManager.AppSettings["ConnectionStringName"];
                if (string.IsNullOrEmpty(connectionStringName))
                {
                    throw new ApplicationException("连接字符串名称未正确配置！");
                }
                string path = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                if (string.IsNullOrEmpty(path))
                {
                    throw new ApplicationException("数据库连接字符串未正确配置！");
                }
                return path;
            }
        }

        /// <summary>
        /// word模板路径
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public static string TemplatePath
        {
            get
            {
                string path = ConfigurationManager.AppSettings["TemplatePath"];
                if (string.IsNullOrEmpty(path))
                {
                    throw new ApplicationException("配置文件中的模板路径未正确配置！");
                }
                return path;
            }
        }

        /// <summary>
        /// 文档输出路径
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        public static string OutputPath
        {
            get
            {
                string path = ConfigurationManager.AppSettings["OutputPath"];
                if (string.IsNullOrEmpty(path))
                {
                    throw new ApplicationException("配置文件中的文档保存路径未正确配置！");
                }
                return path;
            }
        }

        /// <summary>
        /// 是否开启日志
        /// </summary>
        public static bool LogEnabled
        {
            get
            {
                string value = ConfigurationManager.AppSettings["EnableLog"];
                bool result;
                if (bool.TryParse(value, out result))
                {
                    return result;
                }
                return true;
            }
        }
    }
}
