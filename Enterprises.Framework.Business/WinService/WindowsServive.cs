using System;
using System.Collections.Generic;
using System.Timers;
using Enterprises.Framework.Utility;


namespace Enterprises.Framework.Services
{
    /// <summary>
    /// Windows服务类，在客户开发时，有异常可以发送邮件通知相关责任人。快速搭建Windows服务
    /// </summary>
    [Serializable]
    public class WindowsService : System.ServiceProcess.ServiceBase
    {
        private bool _fHasInitServerRemotingObject;
        private readonly object _fHasInitServerRemotingObjectLock = new object();

        private readonly WindowsServiceItemCollection _fTimerServices = new WindowsServiceItemCollection();

        private readonly object _fTimerServiceObjectsLock = new object();
        private readonly Dictionary<Type, ICommand> _fTimerServiceObjects = new Dictionary<Type, ICommand>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="timerServices"></param>
        public WindowsService(WindowsServiceItemCollection timerServices)
        {
            if (timerServices != null)
            {
                foreach (WindowsServiceItem item in timerServices)
                {
                    _fTimerServices.Add(item);
                }
            }
        }

        /// <summary>
        /// 服务启动执行的操作
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            if (_fTimerServices.Count == 0)
            {
                var wsiEmpty = new WindowsServiceItem
                    {
                        WindowsTimer = new WindowsTimer("默认的一个Command的轮询周期，设置为5分钟。", 300000),
                        CommandType = typeof (WindowsServiceEmptyCommand)
                    };
                _fTimerServices.Add(wsiEmpty);
            }
            
            var wsi = new WindowsServiceItem
                {
                    WindowsTimer = new WindowsTimer("默认的一个Command的轮询周期，设置为5秒钟。", 5000),
                    CommandType = typeof (WindowsServiceDefaultCommand)
                };
            _fTimerServices.Add(wsi);

           

            foreach (WindowsServiceItem kvp in _fTimerServices)
            {
                kvp.WindowsTimer.Timer.Elapsed -= Timer_Elapsed;
                kvp.WindowsTimer.Timer.Elapsed += Timer_Elapsed;
                kvp.WindowsTimer.Timer.Enabled = true;
            }
           
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                #region 获取Command对象

                WindowsServiceItem wsItem = _fTimerServices.GetItemByTimer((Timer)sender);
                Type commandType = wsItem.CommandType;
                ICommand command = null;
                if (!_fTimerServiceObjects.ContainsKey(commandType))
                {
                    lock (_fTimerServiceObjectsLock)
                    {
                        if (!_fTimerServiceObjects.ContainsKey(commandType))
                        {
                            var cmd = Activator.CreateInstance(commandType) as ICommand;
                            _fTimerServiceObjects.Add(commandType, cmd);
                            command = cmd;
                        }
                    }
                }
                else
                {
                    command = _fTimerServiceObjects[commandType];
                }

                #endregion

                if (!wsItem.WindowsTimer.Prepared)
                {
                    return;
                }

                if (command != null)
                {
                    lock (wsItem.WindowsTimer)
                    {
                        try
                        {
                            wsItem.WindowsTimer.Prepared = !wsItem.WindowsTimer.Prepared;
                            command.Execute();
                        }
                        catch (Exception ex)
                        {
                            //这里不应该导致整个服务终止，而只是把这个错误信号传递到外面去。
                            if (wsItem.WindowsTimer.CallBack != null)
                            {
                                var args = new WindowsServiceTimerExceptionArgs(ex, e);
                                try
                                {
                                    wsItem.WindowsTimer.CallBack(wsItem.WindowsTimer, args);
                                }
                                catch (Exception ex1)
                                {
                                    LogHelper.WriteErrorLog(ex1.ToString());           
                                }
                            }
                        }
                        finally
                        {
                            wsItem.WindowsTimer.Prepared = !wsItem.WindowsTimer.Prepared;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.ToString());
            }
        }

       

        /// <summary>
        /// 服务停止执行的操作
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                foreach (WindowsServiceItem kvp in _fTimerServices)
                {
                    kvp.WindowsTimer.Timer.Enabled = false;
                    try
                    {
                        kvp.WindowsTimer.Timer.Dispose();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteErrorLog(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.ToString());
            }
        }

        /// <summary>
        /// 释放Timer资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_fTimerServices != null)
                {
                    var timers = new List<WindowsTimer>();
                    foreach (WindowsServiceItem wt in _fTimerServices)
                    {
                        timers.Add(wt.WindowsTimer);
                    }

                    _fTimerServices.Clear();
                    foreach (WindowsTimer wt in timers)
                    {
                        wt.Dispose();
                    }
                }
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// 默认的一个Command，当服务列表中没有注册任何command时，附加上该command对象
    /// </summary>
    internal class WindowsServiceDefaultCommand : ICommand
    {

        public void Execute()
        {
        }
    }

    /// <summary>
    /// 默认的一个Command，当服务列表中没有注册任何command时，附加上该command对象
    /// </summary>
    internal class WindowsServiceEmptyCommand : ICommand
    {

        public void Execute()
        {
        }
    }
}
