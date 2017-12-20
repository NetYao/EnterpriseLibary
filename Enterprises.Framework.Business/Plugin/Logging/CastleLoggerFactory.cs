namespace Enterprises.Framework.Plugin.Logging
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class CastleLoggerFactory : ILoggerFactory
    {
        private readonly Castle.Core.Logging.ILoggerFactory _castleLoggerFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="castleLoggerFactory"></param>
        public CastleLoggerFactory(Castle.Core.Logging.ILoggerFactory castleLoggerFactory)
        {
            _castleLoggerFactory = castleLoggerFactory;
        }

        public ILogger CreateLogger(Type type)
        {
            return new CastleLogger(_castleLoggerFactory.Create(type));
        }
    }
}
