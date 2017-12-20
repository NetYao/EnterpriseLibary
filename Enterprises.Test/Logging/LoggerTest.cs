using System;
using System.Collections.Generic;
using Enterprises.Framework.Plugin.Logging;
using NUnit.Framework;
using log4net.Appender;
using log4net.Core;
using ILogger = Enterprises.Framework.Plugin.Logging.ILogger;

namespace Enterprises.Test.Logging
{
    [TestFixture]
    public class ILoggerTest
    {
        [Test]
        public void TestMethod1()
        {
            IHostEnvironment hostEnvironment= new StubHostEnvironment();
            Castle.Core.Logging.ILoggerFactory cLoggerFactory = new OrchardLog4netFactory(hostEnvironment);
            Framework.Plugin.Logging.ILoggerFactory loggerFactory = new CastleLoggerFactory(cLoggerFactory);
            ILogger log = loggerFactory.CreateLogger(typeof(CastleLogger));
            
            log4net.Config.XmlConfigurator.Configure();
            log.Error("-异常输出{0}-", 42);
            log.Warning(new ApplicationException("测试Warning输出"), "Warning");
            log.Debug(new Exception("测试Debug输出"), "姚立峰");
            // log4net记录在内存中
            //log4net.Config.BasicConfigurator.Configure(new MemoryAppender());
            //Assert.That(log, Is.Not.Null);

            //MemoryAppender.Messages.Clear();
            //log.Error("-boom{0}-", 42);
            //Assert.That(MemoryAppender.Messages, Has.Some.StringContaining("-boom42-"));

            //MemoryAppender.Messages.Clear();
            //log.Warning(new ApplicationException("problem"), "crash");
            //Assert.That(MemoryAppender.Messages, Has.Some.StringContaining("problem"));
            //Assert.That(MemoryAppender.Messages, Has.Some.StringContaining("crash"));
            //Assert.That(MemoryAppender.Messages, Has.Some.StringContaining("ApplicationException"));


        }

        [Test]
        public void TestMethod2()
        {
            ILogger logger = NullLogger.Instance;
            logger.Error("测试异常输出.");
            logger.Warning("测试Warning输出.");
            logger.Debug("测试Debug输出.");
        }
    }

    public class StubHostEnvironment : HostEnvironment
    {
        public override void RestartAppDomain()
        {
        }
    }

    public class MemoryAppender : IAppender
    {
        static MemoryAppender()
        {
            Messages = new List<string>();
        }

        public static List<string> Messages { get; set; }

        public void DoAppend(LoggingEvent loggingEvent)
        {
            if (loggingEvent.ExceptionObject != null)
            {
                lock (Messages) Messages.Add(string.Format("{0} {1} {2}",
                    loggingEvent.ExceptionObject.GetType().Name,
                    loggingEvent.ExceptionObject.Message,
                    loggingEvent.RenderedMessage));
            }
            else lock (Messages) Messages.Add(loggingEvent.RenderedMessage);
        }

        public void Close() { }
        public string Name { get; set; }
    }
}
