using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Enterprises.Framework
{
    /// <summary>
    /// 关注接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConcern<T>
    {
        T This { get; }
    }

    /// <summary>
    /// 连接点
    /// </summary>
    public struct Joinpoint : IEquatable<Joinpoint>
    {
        internal MethodBase PointcutMethod;
        internal MethodBase ConcernMethod;

        private Joinpoint(MethodBase pointcutMethod, MethodBase concernMethod)
        {
            PointcutMethod = pointcutMethod;
            ConcernMethod = concernMethod;
        }

        // Utility method to create joinpoints 
        public static Joinpoint Create(MethodBase pointcutMethod, MethodBase concernMethod)
        {
            return new Joinpoint(pointcutMethod, concernMethod);
        }

        #region IEquatable
        public bool Equals(Joinpoint other)
        {
            return other.ConcernMethod == this.ConcernMethod && other.PointcutMethod == this.PointcutMethod;
        }

        public static bool operator ==(Joinpoint x, Joinpoint y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Joinpoint x, Joinpoint y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || typeof(Joinpoint) != obj.GetType()) return false;
            Joinpoint jp = (Joinpoint)obj;
            return Equals(jp);
        }

        public override int GetHashCode()
        {
            return ConcernMethod.GetHashCode() ^ PointcutMethod.GetHashCode();
        }
        #endregion
    }

    public class AOP : List<Joinpoint>
    {
        static readonly AOP _registry;
        static AOP() { _registry = new AOP(); }
        private AOP() { }
        public static AOP Registry { get { return _registry; } }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Join(MethodBase pointcutMethod, MethodBase concernMethod)
        {
            var joinPoint = Joinpoint.Create(pointcutMethod, concernMethod);
            if (!this.Contains(joinPoint)) this.Add(joinPoint);
        }

        /// <summary>
        /// AopFactory
        /// </summary>
        public static class AopFactory
        {
            public static object Create(Type targetType)
            {
                var joinpoints = AOP.Registry.Where(x => x.PointcutMethod.DeclaringType == targetType).ToArray();
                var concernTypes = joinpoints
                                    .Select(x => x.ConcernMethod.DeclaringType)
                                    .Distinct();

                var allConcerns = concernTypes.Select(x => Activator.CreateInstance(x, null)).ToList();
                return new Interceptor(null, allConcerns.ToArray(), joinpoints).GetTransparentProxy();
            }

            public static object Create<T>(params object[] constructorArgs)
            {
                T target;
                object constructionConcern = null;

                var joinpoints = AOP.Registry.Where(x => x.PointcutMethod.DeclaringType == typeof(T)).ToArray();
                var parametersTypes = constructorArgs.Select(x => x.GetType()).ToArray();

                var concernConstructors = joinpoints
                                            .Where(x =>
                                                x.PointcutMethod.IsConstructor
                                                && Utils.TypeArrayMatch(parametersTypes, x.PointcutMethod.GetParameters().Select(a => a.ParameterType).ToArray()))
                                            .Select(x => x.ConcernMethod)
                                            .ToList();

                if (concernConstructors.Count > 0)
                {
                    if (concernConstructors.Count > 1)
                    {
                        var foundConstructors = String.Join("," + Environment.NewLine, concernConstructors.Select(x => x.DeclaringType.FullName));
                        throw new ApplicationException(String.Format("Creation of type {0} is impossible. Several concerns are claiming its construction: {1}{2}", typeof(T).FullName, Environment.NewLine, foundConstructors));
                    }

                    // create the Concern which claims construction of the target type
                    constructionConcern = Activator.CreateInstance(concernConstructors.First().DeclaringType, constructorArgs);

                    // the This value of the constructionConcern should have been set... let's check
                    target = (T)constructionConcern.GetType().GetProperty("This").GetGetMethod().Invoke(constructionConcern, null);
                    if (target == null)
                    {
                        throw new ApplicationException(String.Format("Construction of type {0} has not been handled properly by Concern: {1}", typeof(T).FullName, constructionConcern.GetType()));
                    }
                }
                else
                {
                    // no Concern is claiming construction with the passed arguments
                    // Let's try to create it directly then
                    target = (T)Activator.CreateInstance(typeof(T), constructorArgs);
                }

                // At that point we have a Target and we might have one Concern already created
                // Let's create the missing ones with a default constructor (no arguments)
                var concernTypes = joinpoints
                                    .Select(x => x.ConcernMethod.DeclaringType)
                                    .Distinct()
                                    .Except(concernConstructors.Select(x => x.DeclaringType));

                var allConcerns = concernTypes.Select(x => Activator.CreateInstance(x, null)).ToList();

                if (constructionConcern != null) allConcerns.Add(constructionConcern);

                // Let's set the This value of the concerns 
                if (target != null)
                {
                    foreach (var concern in allConcerns)
                    {
                        var thisProp = concern.GetType().GetProperty("This");
                        if (thisProp != null)
                        {
                            thisProp.GetSetMethod().Invoke(concern, new object[] { target });
                        }
                    }
                }

                // We have the joinpoints, the target and the concerns
                // we can build the proxy for future call interceptions

                return new Interceptor(target, allConcerns.ToArray(), joinpoints).GetTransparentProxy();
            }

            public static object GetProxy<T>(object target = null)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 拦截器
    /// </summary>
    public sealed class Interceptor : RealProxy, IRemotingTypeInfo
    {
        object Target { get; set; }
        object[] Concerns { get; set; }
        Joinpoint[] Joinpoints { get; set; }

        internal Interceptor(object target, object[] concerns, Joinpoint[] joinpoints)
            : base(typeof(MarshalByRefObject))
        {
            Target = target;
            Concerns = concerns;
            Joinpoints = joinpoints;
        }

        public string TypeName { get; set; }

        public bool CanCastTo(Type fromType, object o) { return true; }

        public override System.Runtime.Remoting.Messaging.IMessage Invoke(System.Runtime.Remoting.Messaging.IMessage msg)
        {
            object returnValue = null;
            IMethodCallMessage methodMessage = (IMethodCallMessage)msg;
            MethodBase method = methodMessage.MethodBase;

            var concernMethod = Joinpoints
                .Where(
                    x =>
                        x.PointcutMethod.Name == method.Name
                        && Utils.TypeArrayMatch(x.PointcutMethod.GetParameters().Select(p => p.ParameterType).ToArray(), method.GetParameters().Select(p => p.ParameterType).ToArray())
                    )
                    .Select(x => x.ConcernMethod).FirstOrDefault();

            if (concernMethod != null)
            {
                var param = concernMethod.IsStatic ? null : Concerns.First(x => x.GetType() == concernMethod.DeclaringType);
                returnValue = concernMethod.Invoke(param, methodMessage.Args);
            }
            else
            {
                var targetMethod = Target
                    .GetType()
                    .GetMethods()
                    .Where(
                        x =>
                            x.Name == method.Name
                            && Utils.TypeArrayMatch(x.GetParameters().Select(p => p.ParameterType).ToArray(), method.GetParameters().Select(p => p.ParameterType).ToArray())
                        )
                        .FirstOrDefault();

                var param = targetMethod.IsStatic ? null : Target;

                returnValue = targetMethod.Invoke(param, methodMessage.Args);
            }

            return new ReturnMessage(returnValue, methodMessage.Args, methodMessage.ArgCount, methodMessage.LogicalCallContext, methodMessage);

        }
    }

    public static class Utils
    {
        public static Func<Type[], Type[], bool> TypeArrayMatch = (x, y) =>
        {
            for (int i = 0; i < x.Length; ++i) { if (x[i] != y[i]) return false; }
            return true;
        };

    }
}
