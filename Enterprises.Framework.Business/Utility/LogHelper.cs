using System;
using System.IO;

namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fileName"></param>
        public static void WriteLog(string text, string fileName)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory + @"\Log";
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            try
            {
                var strW = new StreamWriter(appPath + @"\" + fileName + ".log", true);
                strW.WriteLine("{0} \r\n", text);
                strW.Flush();
                strW.Close();
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public static void WriteLog(string text)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory + @"\Log";
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            try
            {
                var strW = new StreamWriter(appPath + @"\log.log", true);
                strW.WriteLine("{0} \r\n", text);
                strW.Flush();
                strW.Close();
            }
            catch { }
        }

        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="text"></param>
        public static void WriteErrorLog(string text)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory + @"\Log";
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            try
            {
                var strW = new StreamWriter(appPath + @"\Error.log", true);
                strW.WriteLine("{0} \r\n", text);
                strW.Flush();
                strW.Close();
            }
            catch { }
        }

        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="text"></param>
        public static void WriteErrorLog(string text,Exception ex)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory + @"\Log";
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            try
            {
                var strW = new StreamWriter(appPath + @"\Error.log", true);
                strW.WriteLine("{0} \r\n", text);
                strW.WriteLine("{0} \r\n", ex);
                strW.Flush();
                strW.Close();
            }
            catch { }
        }
    }
}
