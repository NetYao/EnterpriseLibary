using System.Configuration;
using Castle.Core.Logging;
using log4net;
using log4net.Config;
using System;

namespace Enterprises.Framework.Plugin.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class OrchardLog4netFactory : AbstractLoggerFactory
    {
        private static bool _isFileWatched = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostEnvironment"></param>
        public OrchardLog4netFactory(IHostEnvironment hostEnvironment)
            : this(ConfigurationManager.AppSettings["log4net.Config"], hostEnvironment) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configFilename"></param>
        /// <param name="hostEnvironment"></param>
        public OrchardLog4netFactory(string configFilename, IHostEnvironment hostEnvironment)
        {
            if (!_isFileWatched && !string.IsNullOrWhiteSpace(configFilename) && hostEnvironment.IsFullTrust)
            {
                // Only monitor configuration file in full trust
                XmlConfigurator.ConfigureAndWatch(GetConfigFile(configFilename));
                _isFileWatched = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override Castle.Core.Logging.ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Castle.Core.Logging.ILogger Create(string name)
        {
            return new OrchardLog4netLogger(LogManager.GetLogger(name), this);
        }
    }
}

