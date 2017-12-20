namespace Enterprises.Framework.Plugin.Logging
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        bool IsEnabled(Plugin.Logging.LogLevel level);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="exception"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Log(Plugin.Logging.LogLevel level, Exception exception, string format, params object[] args);
    }
}

