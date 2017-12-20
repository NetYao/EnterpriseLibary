namespace Enterprises.Framework.Plugin.Logging
{
    using System;

    internal class NullLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger(Type type)
        {
            return NullLogger.Instance;
        }
    }
}

