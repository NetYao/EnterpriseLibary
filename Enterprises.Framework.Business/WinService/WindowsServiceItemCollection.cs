using System;
using System.Collections;
using System.Timers;

namespace Enterprises.Framework.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class WindowsServiceItem
    {
        /// <summary>
        /// 
        /// </summary>
        public WindowsTimer WindowsTimer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type CommandType { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WindowsServiceItemCollection : CollectionBase
    {
        internal static bool IsWindowsServiceEnviroments = false;

        /// <summary>
        /// 
        /// </summary>
        public WindowsServiceItemCollection()
        {
            IsWindowsServiceEnviroments = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(WindowsServiceItem item)
        {
            IsWindowsServiceEnviroments = true;
            List.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Add(WindowsTimer windowsTimer, Type commandType)
        {
            if (windowsTimer == null)
            {
                throw new ArgumentNullException("windowsTimer");
            }

            if (commandType == null)
            {
                throw new ArgumentNullException("commandType");
            }

            Add(new WindowsServiceItem { WindowsTimer = windowsTimer, CommandType = commandType });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Remove(WindowsServiceItem item)
        {
            IsWindowsServiceEnviroments = true;
            if (List.Contains(item))
            {
                List.Remove(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(WindowsServiceItem item)
        {
            return List.Contains(item);
        }

        /// <summary>
        /// 索引集合的对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public WindowsServiceItem this[int index]
        {
            get
            {
                if (index >= 0 && index < List.Count)
                {
                    return List[index] as WindowsServiceItem;
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tm"></param>
        /// <returns></returns>
        public WindowsServiceItem GetItemByTimer(Timer tm)
        {
            foreach (WindowsServiceItem item in this.List)
            {
                if (item.WindowsTimer == null)
                {
                    continue;
                }

                if (item.WindowsTimer.Timer == tm)
                {
                    return item;
                }
            }
            return null;
        }
    }


    /// <summary>
    /// WindowsTimer
    /// </summary>
    public class WindowsTimer : IDisposable
    {
        /// <summary>
        /// 周期任务描述
        /// </summary>
        public string Description = string.Empty;

        /// <summary>
        /// 时钟周期控件
        /// </summary>
        public Timer Timer = new Timer();

        /// <summary>
        /// 是否准备好了可以接受下一个周期的任务
        /// </summary>
        public bool Prepared = true;

        /// <summary>
        /// Windows服务的时间控件异常回调委托
        /// </summary>
        public WindowsServiceTimerExceptionCallBack CallBack;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="description">周期任务描述</param>
        /// <param name="interval">执行周期间隔毫秒数</param>
        public WindowsTimer(string description, double interval)
        {
            Description = description;
            Timer.Interval = interval;
            Timer.Enabled = false;
        }

        #region IDisposable Members

        /// <summary>
        /// 析构函数
        /// </summary>
        ~WindowsTimer()
        {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Timer != null)
                {
                    Timer.Dispose();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Windows服务的时间控件异常回调参数
    /// </summary>
    [Serializable]
    public class WindowsServiceTimerExceptionArgs : EventArgs
    {
        private readonly Exception _fException;
        private readonly ElapsedEventArgs _fElapsedEventArgs;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="args"></param>
        public WindowsServiceTimerExceptionArgs(Exception ex, ElapsedEventArgs args)
        {
            _fException = ex;
            _fElapsedEventArgs = args;
        }

        /// <summary>
        /// 异常对象
        /// </summary>
        public Exception Exception
        {
            get
            {
                return _fException;
            }
        }

        /// <summary>
        /// 时钟参数
        /// </summary>
        public ElapsedEventArgs ElapsedEventArgs
        {
            get
            {
                return _fElapsedEventArgs;
            }
        }
    }

    /// <summary>
    /// Windows服务的时间控件异常回调委托
    /// </summary>
    public delegate void WindowsServiceTimerExceptionCallBack(WindowsTimer windowsTimer, WindowsServiceTimerExceptionArgs e);


}
