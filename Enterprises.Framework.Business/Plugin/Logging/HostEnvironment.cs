namespace Enterprises.Framework.Plugin.Logging
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Hosting;

    public abstract class HostEnvironment : IHostEnvironment
    {
        protected HostEnvironment()
        {
        }

        public bool IsAssemblyLoaded(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Any<Assembly>(delegate (Assembly assembly) {
                return (new AssemblyName(assembly.FullName).Name == name);
            });
        }

        public string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }

        public abstract void RestartAppDomain();

        public bool IsFullTrust
        {
            get
            {
                return (AppDomain.CurrentDomain.IsHomogenous && AppDomain.CurrentDomain.IsFullyTrusted);
            }
        }
    }
}

