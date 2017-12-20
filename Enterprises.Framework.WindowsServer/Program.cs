using System;
using Enterprises.Framework.Services;
using System.IO;


namespace Enterprises.Framework.WindowsServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                var timerServices = new WindowsServiceItemCollection();

                RegisterSendSmsService(timerServices);
                RegisterSynDateService(timerServices);
                WindowsServiceHelper.RunServices(timerServices, WindowsServiceTimerExceptionCallBack);
             
            }
            catch(Exception ex)
            {
                WriterLog(ex.ToString(), "error");
            }

        }

        private static void RegisterSynDateService(WindowsServiceItemCollection timerServices)
        {
            try
            {
                const double interval = 1000;
                var sendTimer = new WindowsTimer("SendSynDataInterval", interval);
                timerServices.Add(sendTimer, typeof(SysDataToDataBase));
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                WriterLog(ex.ToString(), "error");
            }
        }

        static void RegisterSendSmsService(WindowsServiceItemCollection timerServices)
        {
            #region 发送短信服务
            try
            {
                const double interval = 1000;
                var sendTimer = new WindowsTimer("SendSMSInterval", interval);
                timerServices.Add(sendTimer, typeof(SendSmsCommand));
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                WriterLog(ex.ToString(), "error");
            }
            #endregion
        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e != null && e.ExceptionObject != null)
            {
                Console.Write(e.ExceptionObject.ToString());
                WriterLog(e.ExceptionObject.ToString(), "error");
            }
        }

        /// <summary>
        /// 时钟事件报错后执行的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void WindowsServiceTimerExceptionCallBack(object sender, WindowsServiceTimerExceptionArgs args)
        {
            if (args != null)
            {
                WriterLog(args.Exception.Message, "error");
            }
        }

        /// <summary>
        /// 写入日志文件。
        /// </summary>
        public static void WriterLog(string text, string fileName)
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
            catch
            { }
        }
    }
}
