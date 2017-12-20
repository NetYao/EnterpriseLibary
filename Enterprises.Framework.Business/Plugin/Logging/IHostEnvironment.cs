namespace Enterprises.Framework.Plugin.Logging
{
    using System;

    public interface IHostEnvironment
    {
        bool IsAssemblyLoaded(string name);
        string MapPath(string virtualPath);
        void RestartAppDomain();

        bool IsFullTrust { get; }
    }
}

