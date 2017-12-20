using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Module = Autofac.Module;
/*
LoggingModule也是一个Autofac模块。它以属性注入的方式给需要日志服务的对象设置Logger。

如果一个类有ILogger型的公共可写实例属性(忽略索引)，Autofac容器在解析(Resolve)该类的时候，将"注入"一个ILogging实现类的实例。Orchard默认会注入一个CastleLogger对象。当然Orchard也允许一个类中有多个ILogger型属性，也支持将CastleLogger替换成其他Logger。

如有必要，请先了解一下简单工厂模式、抽象工厂模式和适配器模式(对象适配器模式)。CastleLoggerFactory工厂负责创建CastleLogger对象，而CastleLoggerFactory适配了OrchardLog4netFactory；CastleLogger实际上适配的是OrchardLog4netLogger；OrchardLog4netLogger又适配了log4net.Core.ILogger——也就是说CastleLogger是log4net.Core.ILogger经过层层包装的结果。
如果有类TestContrller,其有一个ILogger属性，注入流程如下：
1、Autofac容器创建TestController类的实例instance;
2、检查instance是否有一个ILogger型的公共可写实例属性。如果有（有可能还不只一个）则进入步骤3，否则以下步骤不没必要进行了;
3、从缓存中获取TestController类型对应的Logger，如果获取成功进入步骤6;
4、从Autofac容器获取ILogger：
从容器中获取CastleLoggerFactory
->CastleLoggerFactory创建ILogger对象
->交由OrchardLog4netFactory创建OrchardLog4netLogger
->交由log4net.LogManager.GetLogger创建一个log4net.Core.ILogger对象
->log4net.Core.ILogger对象适配成OrchardLog4netLogger对象
->OrchardLog4netLogger适配成CastleLogger对象
->CastleLoggerFactory最终返回一个CastleLogger对象
5、缓存Logger
6、将Logger注入对应的属性。
 */

namespace Enterprises.Framework.Plugin.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class LoggingModule : Module
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggerCache;

        /// <summary>
        /// 
        /// </summary>
        public LoggingModule()
        {
            _loggerCache = new ConcurrentDictionary<string, ILogger>();
        }

        protected override void Load(ContainerBuilder moduleBuilder)
        {
            // by default, use Orchard's logger that delegates to Castle's logger factory
            moduleBuilder.RegisterType<CastleLoggerFactory>().As<ILoggerFactory>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<OrchardLog4netFactory>()
                         .As<Castle.Core.Logging.ILoggerFactory>()
                         .InstancePerLifetimeScope();

            // call CreateLogger in response to the request for an ILogger implementation
            moduleBuilder.Register(CreateLogger).As<ILogger>().InstancePerDependency();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry,
                                                              IComponentRegistration registration)
        {
            var implementationType = registration.Activator.LimitType;

            // build an array of actions on this type to assign loggers to member properties
            var injectors = BuildLoggerInjectors(implementationType).ToArray();

            // if there are no logger properties, there's no reason to hook the activated event
            if (!injectors.Any())
                return;

            // otherwise, whan an instance of this component is activated, inject the loggers on the instance
            registration.Activated += (s, e) =>
                {
                    foreach (var injector in injectors)
                        injector(e.Context, e.Instance);
                };
        }

        private IEnumerable<Action<IComponentContext, object>> BuildLoggerInjectors(Type componentType)
        {
            // Look for settable properties of type "ILogger" 
            var loggerProperties = componentType
                .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new
                    {
                        PropertyInfo = p,
                        p.PropertyType,
                        IndexParameters = p.GetIndexParameters(),
                        Accessors = p.GetAccessors(false)
                    })
                .Where(x => x.PropertyType == typeof (ILogger)) // must be a logger
                .Where(x => !x.IndexParameters.Any()) // must not be an indexer
                .Where(x => x.Accessors.Length != 1 || x.Accessors[0].ReturnType == typeof (void));
                //must have get/set, or only set

            // Return an array of actions that resolve a logger and assign the property
            foreach (var entry in loggerProperties)
            {
                var propertyInfo = entry.PropertyInfo;

                yield return (ctx, instance) =>
                    {
                        string component = componentType.ToString();
                        var logger = _loggerCache.GetOrAdd(component,
                                                           key =>
                                                           ctx.Resolve<ILogger>(new TypedParameter(typeof (Type),
                                                                                                   componentType)));
                        propertyInfo.SetValue(instance, logger, null);
                    };
            }
        }

        private static ILogger CreateLogger(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            // return an ILogger in response to Resolve<ILogger>(componentTypeParameter)
            var loggerFactory = context.Resolve<ILoggerFactory>();
            var containingType = parameters.TypedAs<Type>();
            return loggerFactory.CreateLogger(containingType);
        }
    }
}

