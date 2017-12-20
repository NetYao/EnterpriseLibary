using System;
using System.ServiceProcess;
using Enterprises.Framework.Utility;

namespace Enterprises.Framework.Services
{
    /// <summary>
    /// Windows服务辅助类
    /// </summary>
    [Serializable]
    public class WindowsServiceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        static WindowsServiceHelper()
        {
            
        }

        /// <summary>
        /// 执行Windows服务
        /// </summary>
        public static void RunServices(WindowsServiceItemCollection timerServices, WindowsServiceTimerExceptionCallBack callBack)
        {
            WindowsServiceItemCollection.IsWindowsServiceEnviroments = true;

            try
            {
                if (timerServices == null)
                {
                    throw new ArgumentNullException("timerServices");
                }

                if (callBack != null)
                {
                    foreach (WindowsServiceItem kvp in timerServices)
                    {
                        kvp.WindowsTimer.CallBack += callBack;
                    }
                }

                var servicesToRun = new ServiceBase[]
                { 
                    new WindowsService(timerServices)            
                };

                foreach (ServiceBase service in servicesToRun)
                {
                    service.AutoLog = true;
                }

                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                ServiceBase.Run(servicesToRun);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.ToString());                  
                throw;
            }
        }

        /// <summary>
        /// 未处理异常的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    if (e.ExceptionObject != null)
                    {
                        if (e.ExceptionObject is Exception)
                        {
                            LogHelper.WriteErrorLog(e.ExceptionObject.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.ToString());           
            }
        }
    }

    /// <summary>
    /// Windows Service 服务接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        ///  执行方法
        /// </summary>
        void Execute();
    }
}
