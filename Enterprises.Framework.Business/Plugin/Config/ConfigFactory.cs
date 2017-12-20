using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Enterprises.Framework.Plugin.Config
{
    /// <summary>
    /// 针对配置文件的工厂类，提供文件的缓存
    /// </summary>
    public class ConfigFactory
    {
        private static readonly object FLockConfigHelpers = new object();
        private static readonly Dictionary<string, ConfigBase> FConfigHelpers = new Dictionary<string, ConfigBase>();

        private ConfigFactory()
        {
        }

        /// <summary>
        /// 获取配置文件帮助对象。
        /// </summary>
        /// <param name="configFileName">配置文件名称（包含扩展名）</param>
        /// <param name="configFilesPath">配置文件所在路径</param>
        /// <param name="reloadCache">是否更新缓存</param>
        /// <returns></returns>
        public static ConfigBase Create(string configFileName, string configFilesPath="", bool reloadCache = false)
        {
            if (string.IsNullOrEmpty(configFilesPath))
            {
                configFilesPath = ConfigBase.GetWebOrAppConfigPath();
            }

            configFileName = Path.Combine(configFilesPath, configFileName);
            string cachedName = string.Format("{0}", configFileName.ToUpper().Trim());

            //每次重新加载配置文件
            if (reloadCache)
            {
                //因为重新加载，所以把原来的删除，后面再增加新的
                if (FConfigHelpers.ContainsKey(cachedName))
                {
                    FConfigHelpers.Remove(cachedName);
                }
            }

            //不需要重新加载
            if (!FConfigHelpers.ContainsKey(cachedName))
            {
                lock (FLockConfigHelpers)
                {
                    if (!FConfigHelpers.ContainsKey(cachedName))
                    {
                        var chb = new ConfigBase(configFileName);
                        FConfigHelpers.Add(cachedName, chb);
                    }
                }
            }

            return FConfigHelpers[cachedName];
        }
    }
}
