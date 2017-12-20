namespace Enterprises.Framework.Plugin.Logging
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class NullLogger : ILogger
    {
        private static readonly ILogger _instance = new NullLogger();

        public bool IsEnabled(LogLevel level)
        {
            return false;
        }

        public void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public static ILogger Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}

